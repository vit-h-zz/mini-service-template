using System.Threading;
using System.Threading.Tasks;
using Common.Errors;
using Messaging.MiniService;
using Messaging.MiniService.Enums;
using MiniService.Data.Domain.Entities;
using MiniService.Data.Persistence;
using FluentResults;
using Mapster;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MiniService.Application.TodoItems.UpdateTodoItem
{
    public class UpdateTodoItemHandler : IRequestHandler<UpdateTodoItemCommand, Result>
    {
        private readonly MiniServiceDbContext _db;
        private readonly IPublishEndpoint _publisher;

        public UpdateTodoItemHandler(MiniServiceDbContext db,
            IPublishEndpoint publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        public async Task<Result> Handle(UpdateTodoItemCommand r, CancellationToken ct)
        {
            var canUpdate = await _db.TodoItems.AnyAsync(x => x.Id == r.Id && !x.Done, ct);

            if (!canUpdate) return Result.Fail(new NotFoundError<TodoItem, UpdateTodoItemCommand>());

            var item = r.Adapt<TodoItem>();

            _db.Update(item);

            await _db.SaveChangesAsync(ct);

            if (item.Done) await _publisher.Publish(item.Adapt<WorkDone>(), ct);
            else await _publisher.Publish(new TodoUpdated(item, DataChangeType.Updated), ct);

            return Result.Ok();
        }
    }
}
