using System.Threading;
using System.Threading.Tasks;
using VH.MiniService.Common.Application.Abstractions;
using VH.MiniService.Common.Data.Extensions;
using TemplateService.Data.Domain.Entities;
using TemplateService.Data.Persistence;
using VH.MiniService.Common.Errors;
using FluentResults;
using MassTransit;
using MediatR;
using Messaging.TemplateService;
using VH.MiniService.Messaging.Common.Enums;

namespace TemplateService.Application.TodoItems.Delete
{
    public class DeleteTodoItemHandler : IRequestHandler<DeleteTodoItemCommand, Result>
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publisher;
        private readonly IUserContext _userContext;

        public DeleteTodoItemHandler(AppDbContext db, IPublishEndpoint publisher, IUserContext userContext)
        {
            _db = db;
            _publisher = publisher;
            _userContext = userContext;
        }

        public async Task<Result> Handle(DeleteTodoItemCommand r, CancellationToken ct)
        {
            var userId = _userContext.GetUserId();

            var entity = await _db.TodoItems.FindByKeyAsync(r.Id, ct);

            if (entity == null)
                return new NotFoundError<TodoItem, DeleteTodoItemCommand>();

            _db.TodoItems.Remove(entity);

            await _db.SaveChangesAsync(ct);

            await _publisher.Publish(new TodoUpdated(entity, EventType.Deleted, userId), CancellationToken.None);

            return Result.Ok();
        }
    }
}
