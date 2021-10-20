using System.Threading;
using System.Threading.Tasks;
using Common.Application.Abstractions;
using Common.Errors;
using FluentResults;
using Mapster;
using MassTransit;
using MediatR;
using Messaging.Common.Enums;
using Messaging.MiniService;
using Microsoft.EntityFrameworkCore;
using MiniService.Data.Domain.Entities;
using MiniService.Data.Persistence;

namespace MiniService.Application.TodoItems.Update
{
    public class UpdateTodoItemHandler : IRequestHandler<UpdateTodoItemCommand, Result>
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publisher;
        private readonly IUserContext _userContext;

        public UpdateTodoItemHandler(AppDbContext db, IPublishEndpoint publisher, IUserContext userContext)
        {
            _db = db;
            _publisher = publisher;
            _userContext = userContext;
        }

        public async Task<Result> Handle(UpdateTodoItemCommand r, CancellationToken ct)
        {
            var userId = _userContext.GetUserId();

            var canUpdate = await _db.TodoItems.AnyAsync(x => x.Id == r.Id && !x.Done, ct);

            if (!canUpdate) return Result.Fail(new NotFoundError<TodoItem, UpdateTodoItemCommand>());

            var item = r.Adapt<TodoItem>();

            _db.Update(item);

            await _db.SaveChangesAsync(ct);

            if (item.Done) await _publisher.Publish(item.Adapt<WorkDone>(), CancellationToken.None);
            else await _publisher.Publish(new TodoUpdated(item, EventType.Updated, userId), CancellationToken.None);

            return Result.Ok();
        }
    }
}
