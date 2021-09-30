using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Tests;
using Messaging.MiniService;
using Messaging.MiniService.Enums;
using MiniService.Application.TodoItems.CreateTodoItem;
using MiniService.Application.TodoItems.GetFinishedWork;
using MiniService.Application.TodoItems.UpdateTodoItem;
using MiniService.Data.Domain.Entities;
using MiniService.Data.Persistence;
using MiniService.Features.Todos;
using MiniService.Tests.Fakers;
using MiniService.Tests.Helpers;
using FluentAssertions;
using FluentResults;
using Mapster;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MiniService.Tests.Integration
{
    public class TodoItemsApiTests : IAsyncDisposable
    {
        private const string ToDoUrl = "api/Todos";
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly InMemoryTestHarness _harness;
        private readonly MiniServiceDbContext _dbContext;
        private readonly IServiceScope _scope;
        private readonly HttpClient _client;

        public TodoItemsApiTests()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();

            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<MiniServiceDbContext>();

            _harness = _factory.Services.GetRequiredService<InMemoryTestHarness>();
            _harness.SetupMassTransitTestHarnessMiddleware(_factory.Services);
            //_harness.SetupMassTransitTestHarnessErrorMiddleware();

            _harness.Start();
        }

        [Theory, AutoMoqData]
        public async Task CreateTodoByConsumer(ToDoItemNewFaker itemFaker)
        {
            // arrange
            var requestClient = _harness.Bus.CreateRequestClient<CreateTodoItemCommand>();
            var cmd = itemFaker.Generate().Adapt<CreateTodoItemCommand>();

            // act
            var response = await requestClient.GetResponse<Result<TodoItem>>(cmd);

            // assert
            response.Message.Value.Title.Should().Be(cmd.Title);
            response.Message.Value.Priority.Should().Be(cmd.Priority);

            var consumerHarness = _scope.ServiceProvider.GetRequiredService<IConsumerTestHarness<TodosConsumer>>();
            consumerHarness.Consumed.Select<CreateTodoItemCommand>().Should().HaveCount(1);
            _harness.Published.Select<TodoUpdated>().Should().HaveCount(1);
        }

        [Theory, AutoMoqData]
        public async Task GetTodoItems(ToDoItemExistingFaker existingFaker)
        {
            // arrange
            _dbContext.TodoItems.AddRange(existingFaker.Generate(2));
            await _dbContext.SaveChangesAsync();

            // act
            var apiResponse = await _client.GetAsync("api/Todos");

            // assert
            await apiResponse.IsSucceed();

            var items = await apiResponse.Content.ReadFromJsonAsync<List<TodoItem>>();
            items.Should().NotBeNull();
            items.Should().HaveCount(2);
        }

        [Theory, AutoMoqData]
        public async Task CreateToDoItem(ToDoItemNewFaker itemNewFaker)
        {
            //act
            var cmd = itemNewFaker.Generate().Adapt<CreateTodoItemCommand>();
            var apiResponse = await _client.PostAsJsonAsync(ToDoUrl, cmd);

            // assert
            await apiResponse.IsSucceed();
            apiResponse = await _client.GetAsync(ToDoUrl);
            apiResponse.IsSuccessStatusCode.Should().BeTrue();
            var toDoItems = await apiResponse.Content.ReadFromJsonAsync<List<TodoItem>>();

            toDoItems.Should().NotBeNull();
            toDoItems.Should().HaveCount(1);
        }

        [Theory, AutoMoqData]
        public async Task UpdateTodoItem(ToDoItemExistingFaker existingFaker)
        {
            //arrange
            var t1 = existingFaker.Generate();
            var t2 = existingFaker.Generate();
            _dbContext.Add(t1);
            _dbContext.Add(t2);
            await _dbContext.SaveChangesAsync();

            var cmd = t1.Adapt<UpdateTodoItemCommand>();
            cmd.Title = "Test Title";
            cmd.Priority = PriorityLevel.Medium;

            var t2inDb = t2.Adapt<TodoItem>();

            //act
            var apiResponse = await _client.PutAsJsonAsync($"{ToDoUrl}/{t1.Id}", cmd);

            //assert
            await apiResponse.IsSucceed();

            _dbContext.Entry(t1).Reload();
            t1.Title.Should().Be(cmd.Title);
            t1.Priority.Should().Be(cmd.Priority);

            _dbContext.Entry(t2).Reload();
            t2inDb.Should().BeEquivalentTo(t2);
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

            var apiResponse = await _client.PutAsJsonAsync($"{ToDoUrl}/{t1.Id}", cmd1);
            apiResponse.IsSuccessStatusCode.Should().BeTrue(await apiResponse.Content.ReadAsStringAsync());

            //act
            apiResponse = await _client.PutAsJsonAsync($"{ToDoUrl}/{t1.Id}", cmd2);

            //assert
            apiResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError); // Need to handle concurrency exception properly
            _dbContext.Entry(t1).Reload();
            t1.Title.Should().Be(cmd1.Title);
        }

        [Theory, AutoMoqData]
        public async Task DeleteTodoItemCommand(ToDoItemExistingFaker existingFaker, ToDoItemDoneFaker doneFaker)
        {
            //arrange
            var td1 = existingFaker.Generate();
            var td2 = existingFaker.Generate();
            var td3 = doneFaker.Generate();
            _dbContext.Add(td1);
            _dbContext.Add(td2);
            _dbContext.Add(td3);
            await _dbContext.SaveChangesAsync();

            // act
            var apiResponse = await _client.DeleteAsync(ToDoUrl + $"/{td1.Id}");

            // assert
            await apiResponse.IsSucceed();

            var todItems = _dbContext.TodoItems.ToList();
            todItems.Should().HaveCount(2);
            todItems.Should().Contain(new[] { td2, td3 });
        }

        [Theory, AutoMoqData]
        public async Task GetFinishedWork(ToDoItemExistingFaker existingFaker, ToDoItemDoneFaker doneFaker)
        {
            //arrange
            _dbContext.Add(existingFaker.Generate());
            _dbContext.Add(doneFaker.Generate());
            await _dbContext.SaveChangesAsync();

            //act
            var apiResponse = await _client.GetAsync(ToDoUrl + "/finished".CamelToSentenceCase());

            // assert
            await apiResponse.IsSucceed();

            var items = await apiResponse.Content.ReadFromJsonAsync<List<WorkItem>>();
            items.Should().NotBeNull();
            items.Should().HaveCount(2);
        }

        public async ValueTask DisposeAsync()
        {
            await _harness.Stop();
            await _harness.DisposeAsync();
            await _factory.DisposeAsync();
        }
    }
}
