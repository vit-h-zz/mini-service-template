using System.Threading;
using System.Threading.Tasks;
using VH.MiniService.Common.Application.Abstractions;
using VH.MiniService.Common.Errors;
using VH.MiniService.Messaging.Common.Enums;
using FluentResults;
using Mapster;
using MassTransit;
using MediatR;
using Messaging.TemplateService;
using Microsoft.EntityFrameworkCore;
using TemplateService.Data.Domain.Entities;
using TemplateService.Data.Persistence;

namespace TemplateService.Application.TodoItems.Update
{
    public class UpdateTodoItemHandler : IRequestHandler<UpdateTodoItemCommand, Result>
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publisher;
        private readonly IRequestContext _requestContext;

        public UpdateTodoItemHandler(AppDbContext db, IPublishEndpoint publisher, IRequestContext requestContext)
        {
            _db = db;
            _publisher = publisher;
            _requestContext = requestContext;
        }

        public async Task<Result> Handle(UpdateTodoItemCommand r, CancellationToken ct)
        {
            var userId = _requestContext.UserIdOrThrow;

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
