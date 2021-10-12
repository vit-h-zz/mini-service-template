using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Mapster;
using MassTransit;
using MediatR;
using Messaging.MiniService;
using Messaging.MiniService.Enums;
using MiniService.Data.Domain.Entities;
using MiniService.Data.Persistence;

namespace MiniService.Application.TodoItems.Create
{
    public class CreateTodoItemHandler : IRequestHandler<CreateTodoItemCommand, Result<TodoItem>>
    {
        private readonly AppContext _db;
        private readonly IPublishEndpoint _publisher;

        public CreateTodoItemHandler(AppContext db,
            IPublishEndpoint publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        public async Task<Result<TodoItem>> Handle(CreateTodoItemCommand request, CancellationToken ct)
        {
            var entity = new TodoItem
            {
                Title = request.Title,
                Priority = request.Priority,
                Done = false
            };

            _db.TodoItems.Add(entity);

            var todo = entity.Adapt<TodoItem>();

            await _db.SaveChangesAsync(ct);

            await _publisher.Publish(new TodoUpdated(todo, DataChangeType.Created), CancellationToken.None);

            return Result.Ok(todo);
        }
    }
}
