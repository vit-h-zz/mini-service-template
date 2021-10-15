using System.Threading;
using System.Threading.Tasks;
using FluentResults;
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
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publisher;

        public CreateTodoItemHandler(AppDbContext db,
            IPublishEndpoint publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        public async Task<Result<TodoItem>> Handle(CreateTodoItemCommand r, CancellationToken ct)
        {
            var entity = new TodoItem
            {
                Title = r.Title,
                Priority = r.Priority,
                Done = false
            };

            _db.TodoItems.Add(entity);

            await _db.SaveChangesAsync(ct);

            await _publisher.Publish(new TodoUpdated(entity, DataChangeType.Created), CancellationToken.None);

            return Result.Ok(entity);
        }
    }
}
