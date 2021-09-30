using System;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Messaging.MiniService;
using NodaTime;

namespace MiniService.Features.Todos
{
    public class TodosConsumer
        : IConsumer<CompleteTodoItemCmd>
    {
        private readonly IMediator _mediator;
        private readonly IClock _clock;

        public TodosConsumer(IMediator mediator, IClock clock)
        {
            _mediator = mediator;
            _clock = clock;
        }

        public async Task Consume(ConsumeContext<CompleteTodoItemCmd> context)
        {
            var result = await _mediator.Send(context.Message, context.CancellationToken);

            await context.RespondAsync(new CompleteTodoItemResult()
            {
                ClosingTime = _clock.GetCurrentInstant().ToDateTimeOffset()
            });
        }
    }
}
