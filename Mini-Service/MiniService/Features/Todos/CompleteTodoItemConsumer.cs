using System.Threading.Tasks;
using Mapster;
using MassTransit;
using MediatR;
using Messaging.MiniService;
using MiniService.Application.TodoItems.Complete;

namespace MiniService.Features.Todos
{
    /// <summary>
    /// Consumer for CompleteTodoItemCmd
    /// </summary>
    // More info regarding routing (https://www.youtube.com/watch?v=bsUlQ93j2MY)
    // and retry policies (https://www.youtube.com/watch?v=pKxf6Ii-3ow)
    public class CompleteTodoItemConsumer : IConsumer<CompleteTodoItemCmd>
    {
        private readonly IMediator _mediator;

        public CompleteTodoItemConsumer(IMediator mediator)
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
