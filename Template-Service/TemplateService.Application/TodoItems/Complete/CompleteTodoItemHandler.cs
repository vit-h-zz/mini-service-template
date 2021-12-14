using System.Threading;
using System.Threading.Tasks;
using VH.MiniService.Common.Application.Abstractions;
using VH.MiniService.Common.Errors;
using TemplateService.Data.Domain.Entities;
using TemplateService.Data.Persistence;
using FluentResults;
using Mapster;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using VH.MiniService.Messaging.Common.Enums;
using Messaging.TemplateService;

namespace TemplateService.Application.TodoItems.Complete
{
    public class CompleteTodoItemHandler : IRequestHandler<CompleteTodoItemCommand, Result<CompleteTodoItemResult>>
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publisher;
        private readonly IRequestContext _requestContext;
        private readonly IClock _clock;

        public CompleteTodoItemHandler(AppDbContext db, IPublishEndpoint publisher,
            IClock clock, IRequestContext requestContext)
        {
            _db = db;
            _publisher = publisher;
            _clock = clock;
            _requestContext = requestContext;
        }

        public async Task<Result<CompleteTodoItemResult>> Handle(CompleteTodoItemCommand r, CancellationToken ct)
        {
            var userId = _requestContext.UserIdOrThrow;
            var item = await _db.TodoItems.FirstOrDefaultAsync(x => x.Id == r.Id, ct);

            if (item == null) return Result.Fail(new NotFoundError<TodoItem, CompleteTodoItemCommand>());
            if (item.Done) return Result.Fail(new ConflictError<TodoItem, CompleteTodoItemCommand>("Item already in the done state"));

            item.Done = true;
            _db.Update(item);

            await _db.SaveChangesAsync(ct);

            var workItem = item.Adapt<WorkItem>();

            await _publisher.Publish(new TodoUpdated(item, EventType.Updated, userId), CancellationToken.None);
            await _publisher.Publish(new WorkDone(workItem, userId), CancellationToken.None);

            return Result.Ok(new CompleteTodoItemResult() { ClosingTime = _clock.GetCurrentInstant().ToDateTimeOffset() });
        }
    }
}
