using System.Threading.Tasks;
using Mapster;
using MassTransit;
using MediatR;
using Messaging.MiniService;
using MiniService.Application.TodoItems.Complete;

namespace MiniService.Features.Todos
{
    public class TodosConsumer
        : IConsumer<CompleteTodoItemCmd>
    {
        private readonly IMediator _mediator;

        public TodosConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<CompleteTodoItemCmd> context)
        {
            var result = await _mediator.Send(context.Message.Adapt<CompleteTodoItemCommand>(), context.CancellationToken);

            await context.RespondAsync(result.Value);
        }
    }
}
