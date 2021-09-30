using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MiniService.Data.Domain.Entities;
using MiniService.Data.Persistence;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MiniService.Application.TodoItems.GetTodoItems
{
    public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, Result<List<TodoItem>>>
    {
        private readonly MiniServiceDbContext _db;

        public GetTodoItemsQueryHandler(MiniServiceDbContext db)
        {
            _db = db;
        }

        public async Task<Result<List<TodoItem>>> Handle(GetTodoItemsQuery r, CancellationToken ct)
        {
            return Result.Ok(await _db.TodoItems
                .OrderBy(x => x.Title)
                .ToListAsync(ct));
        }
    }
}
