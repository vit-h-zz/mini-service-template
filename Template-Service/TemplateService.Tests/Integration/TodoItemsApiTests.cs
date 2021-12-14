using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VH.MiniService.Common.Extensions;
using Messaging.TemplateService;
using Messaging.TemplateService.Enums;
using TemplateService.Data.Domain.Entities;
using TemplateService.Data.Persistence;
using TemplateService.Features.Todos;
using TemplateService.Tests.Fakers;
using TemplateService.Tests.Helpers;
using FluentAssertions;
using NodaTime;
using Mapster;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using TemplateService.Application.TodoItems.Create;
using TemplateService.Application.TodoItems.Update;
using VH.MiniService.Common.Tests;
using VH.MiniService.Common;
using Xunit.Abstractions;

namespace TemplateService.Tests.Integration
{
    public class TodoItemsApiTests : IAsyncDisposable
    {
        private const string ToDoUrl = "todos";
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly InMemoryTestHarness _harness;
        private readonly AppDbContext _dbContext;
        private readonly IServiceScope _scope;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output; // Use as a Console.WriteLine() in tests

        public TodoItemsApiTests(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;

            _factory = new CustomWebApplicationFactory<Startup>(_output);
            _client = _factory.CreateClient();
            _client.SetAccessToken(new Dictionary<string, object> { { CommonRequestHeaders.UserId, 111 } });

            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

            _harness = _factory.Services.GetRequiredService<InMemoryTestHarness>();
            _harness.AddMassTransitTestMiddleware(_factory.Services);
            _harness.Start();
            _harness.AddDefaultHeaders((CommonRequestHeaders.UserId, 111));
        }

        [Theory, AutoMoqData]
        public async Task CreateTodoByConsumer(ToDoItemExistingFaker existingFaker)
        {
            // arrange
            var item1 = existingFaker.Generate();
            var item2 = existingFaker.Generate();
            _dbContext.TodoItems.AddRange(item1, item2);
            await _dbContext.SaveChangesAsync();

            var closingTime = new Instant();
            _factory.ClockMock.Setup(_ => _.GetCurrentInstant()).Returns(closingTime);

            var requestClient = _harness.Bus.CreateRequestClient<CompleteTodoItemCmd>();
            var cmd = new CompleteTodoItemCmd() { TodoId = item1.Id };

            // act
            var response = await requestClient.GetResponse<CompleteTodoItemResult>(cmd);

            // assert
            response.Message.ClosingTime.Should().Be(closingTime.ToDateTimeOffset());
            response.Headers.Get<int>("myCustomHeader").Should().Be(123);

            await _dbContext.Entry(item1).ReloadAsync();
            item1.Done.Should().BeTrue();

            await _dbContext.Entry(item2).ReloadAsync();
            item2.Done.Should().BeFalse();

            var consumerHarness = _scope.ServiceProvider.GetRequiredService<IConsumerTestHarness<CompleteTodoItemConsumer>>();
            consumerHarness.Consumed.Select<CompleteTodoItemCmd>().Should().HaveCount(1);
            _harness.Published.Select<WorkDone>().Should().HaveCount(1);
        }

        [Theory, AutoMoqData]
        public async Task GetTodoItems(ToDoItemExistingFaker existingFaker)
        {
            // arrange
            _dbContext.TodoItems.AddRange(existingFaker.Generate(2));
            await _dbContext.SaveChangesAsync();

            // act
            var items = await _client.Get<List<TodoItem>>(ToDoUrl);

            // assert
            items.Should().NotBeNull();
            items.Should().HaveCount(2);
        }

        [Theory, AutoMoqData]
        public async Task CreateToDoItem(ToDoItemNewFaker itemNewFaker)
        {
            //act
            var cmd = itemNewFaker.Generate().Adapt<CreateTodoItemCommand>();
            var toDoItem = await _client.Post<TodoItem>(ToDoUrl, cmd, HttpStatusCode.Created);

            // assert
            var todoItems = _dbContext.TodoItems.ToList();
            todoItems.Should().HaveCount(1);
            todoItems.First().Should().BeEquivalentTo(toDoItem);
        }

        [Theory, AutoMoqData]
        public async Task UpdateTodoItem(ToDoItemExistingFaker existingFaker)
        {
            //arrange
            var t1 = existingFaker.Generate();
            var t2 = existingFaker.Generate();
            _dbContext.TodoItems.AddRange(t1, t2);
            await _dbContext.SaveChangesAsync();

            var cmd = t1.Adapt<UpdateTodoItemCommand>();
            cmd.Title = "Test Title";
            cmd.Priority = PriorityLevel.Medium;

            var t2InDb = t2.Adapt<TodoItem>();

            //act
            await _client.Put($"{ToDoUrl}/{t1.Id}", cmd);

            //assert
            await _dbContext.Entry(t1).ReloadAsync();
            t1.Title.Should().Be(cmd.Title);
            t1.Priority.Should().Be(cmd.Priority);

            await _dbContext.Entry(t2).ReloadAsync();
            t2InDb.Should().BeEquivalentTo(t2);
        }

        [Theory, AutoMoqData]
        public async Task UpdateTodoItem_Concurrency(ToDoItemExistingFaker f1)
        {
            //arrange
            var t1 = f1.Generate();
            _dbContext.Add(t1);
            await _dbContext.SaveChangesAsync();

            var cmd1 = t1.Adapt<UpdateTodoItemCommand>();
            var cmd2 = t1.Adapt<UpdateTodoItemCommand>();

            cmd1.Title = "Test Title 1";
            cmd2.Title = "Test Title 2";

            await _client.Put($"{ToDoUrl}/{t1.Id}", cmd1);

            //act
            await _client.Put($"{ToDoUrl}/{t1.Id}", cmd2, HttpStatusCode.InternalServerError);

            //assert
            await _dbContext.Entry(t1).ReloadAsync();
            t1.Title.Should().Be(cmd1.Title);
        }

        [Theory, AutoMoqData]
        public async Task DeleteTodoItemCommand(ToDoItemExistingFaker existingFaker, ToDoItemDoneFaker doneFaker)
        {
            //arrange
            var td1 = existingFaker.Generate();
            var td2 = existingFaker.Generate();
            var td3 = doneFaker.Generate();
            _dbContext.TodoItems.AddRange(td1, td2, td3);
            await _dbContext.SaveChangesAsync();

            // act
            await _client.Delete(ToDoUrl + $"/{td1.Id}");

            // assert
            var todItems = _dbContext.TodoItems.ToList();
            todItems.Should().HaveCount(2);
            todItems.Should().Contain(new[] { td2, td3 });
        }

        [Theory, AutoMoqData]
        public async Task GetFinishedWork(ToDoItemExistingFaker existingFaker, ToDoItemDoneFaker doneFaker)
        {
            //arrange
            var existing = existingFaker.Generate();
            _dbContext.TodoItems.AddRange(existing, doneFaker.Generate());
            await _dbContext.SaveChangesAsync();

            //act
            var items = await _client.Get<List<WorkItem>>(ToDoUrl + "/finished");

            // assert
            items.Should().NotBeNull();
            items.Should().NotContain(x => x.TodoId == existing.Id);
        }

        public async ValueTask DisposeAsync()
        {
            await _harness.Stop();
            await _harness.DisposeAsync();
            await _factory.DisposeAsync();
        }
    }
}
