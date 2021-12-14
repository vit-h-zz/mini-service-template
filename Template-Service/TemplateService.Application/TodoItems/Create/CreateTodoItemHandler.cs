using System.Threading;
using System.Threading.Tasks;
using VH.MiniService.Common.Application.Abstractions;
using FluentResults;
using MassTransit;
using MediatR;
using VH.MiniService.Messaging.Common.Enums;
using Messaging.TemplateService;
using TemplateService.Data.Domain.Entities;
using TemplateService.Data.Persistence;

namespace TemplateService.Application.TodoItems.Create
{
    public class CreateTodoItemHandler : IRequestHandler<CreateTodoItemCommand, Result<TodoItem>>
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publisher;
        private readonly IRequestContext _requestContext;

        public CreateTodoItemHandler(AppDbContext db, IPublishEndpoint publisher, IRequestContext requestContext)
        {
            _db = db;
            _publisher = publisher;
            _requestContext = requestContext;
        }

        public async Task<Result<TodoItem>> Handle(CreateTodoItemCommand r, CancellationToken ct)
        {
            var userId = _requestContext.UserIdOrThrow;
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
