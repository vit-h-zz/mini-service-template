using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using MassTransit;
using Messaging.MiniService;
using MiniService.Application.TodoItems.Create;
using MiniService.Tests.Helpers;
using Moq;
using Xunit;

namespace MiniService.Tests.Unit.Todo
{
    public class PublishingChecks
    {
        [Theory, AutoMoqWithInMemoryDb]
        public async Task ShouldPublishOnCreate([Frozen] Mock<IPublishEndpoint> publisher, CreateTodoItemHandler handler, CreateTodoItemCommand command)
        {
            // arrange
            // act
            await handler.Handle(command, CancellationToken.None);

            // assert
            publisher.Verify(x => x.Publish(It.IsAny<TodoUpdated>(), CancellationToken.None), Times.Once());
        }
    }
}
