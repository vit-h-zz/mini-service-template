using System.Threading;
using System.Threading.Tasks;
using Common.Application.Abstractions;
using FluentResults;
using MassTransit;
using MediatR;
using Messaging.Common.Enums;
using Messaging.MiniService;
using Messaging.MiniService.Enums;
using MiniService.Data.Domain.Entities;
using MiniService.Data.Persistence;

namespace MiniService.Application.TodoItems.Create
{
    public class CreateTodoItemHandler : IRequestHandler<CreateTodoItemCommand, Result<TodoItem>>
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publisher;
        private readonly IUserContext _userContext;

        public CreateTodoItemHandler(AppDbContext db, IPublishEndpoint publisher, IUserContext userContext)
        {
            _db = db;
            _publisher = publisher;
            _userContext = userContext;
        }

        public async Task<Result<TodoItem>> Handle(CreateTodoItemCommand r, CancellationToken ct)
        {
            var userId = _userContext.GetUserId();
            var entity = new TodoItem
            {
                Title = r.Title,
                Priority = r.Priority,
                Done = false
            };

            _db.TodoItems.Add(entity);

            await _db.SaveChangesAsync(ct);

            await _publisher.Publish(new TodoUpdated(entity, EventType.Created, userId), CancellationToken.None);

            return Result.Ok(entity);
        }
    }
}
