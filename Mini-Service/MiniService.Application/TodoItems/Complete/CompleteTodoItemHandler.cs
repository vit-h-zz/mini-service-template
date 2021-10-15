using System.Threading;
using System.Threading.Tasks;
using Common.Errors;
using Messaging.MiniService;
using MiniService.Data.Domain.Entities;
using MiniService.Data.Persistence;
using FluentResults;
using Mapster;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace MiniService.Application.TodoItems.Complete
{
    public class CompleteTodoItemHandler : IRequestHandler<CompleteTodoItemCommand, Result<CompleteTodoItemResult>>
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publisher;
        private readonly IClock _clock;

        public CompleteTodoItemHandler(AppDbContext db,
            IPublishEndpoint publisher, IClock clock)
        {
            _db = db;
            _publisher = publisher;
            _clock = clock;
        }

        public async Task<Result<CompleteTodoItemResult>> Handle(CompleteTodoItemCommand r, CancellationToken ct)
        {
            var item = await _db.TodoItems.FirstOrDefaultAsync(x => x.Id == r.Id, ct);

            if (item == null) return Result.Fail(new NotFoundError<TodoItem, CompleteTodoItemCommand>());
            if (item.Done) return Result.Fail(new ConflictError<TodoItem, CompleteTodoItemCommand>("Item already in the done state"));

            item.Done = true;
            _db.Update(item);

            await _db.SaveChangesAsync(ct);

            await _publisher.Publish(item.Adapt<WorkDone>(), ct);

            return Result.Ok(new CompleteTodoItemResult() { ClosingTime = _clock.GetCurrentInstant().ToDateTimeOffset() });
        }
    }
}
