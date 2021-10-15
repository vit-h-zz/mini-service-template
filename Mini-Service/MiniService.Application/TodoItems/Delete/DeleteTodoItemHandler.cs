using System.Threading;
using System.Threading.Tasks;
using Common.Data.Extensions;
using MiniService.Data.Domain.Entities;
using MiniService.Data.Persistence;
using Common.Errors;
using FluentResults;
using MediatR;

namespace MiniService.Application.TodoItems.Delete
{
    public class DeleteTodoItemHandler : IRequestHandler<DeleteTodoItemCommand, Result>
    {
        private readonly AppDbContext _db;

        public DeleteTodoItemHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(DeleteTodoItemCommand r, CancellationToken ct)
        {
            var entity = await _db.TodoItems.FindByKeyAsync(r.Id, ct);

            if (entity == null)
                return new NotFoundError<TodoItem, DeleteTodoItemCommand>();

            _db.TodoItems.Remove(entity);

            await _db.SaveChangesAsync(ct);

            return Result.Ok();
        }
    }
}
