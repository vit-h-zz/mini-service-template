using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TemplateService.Data.Domain.Entities;
using TemplateService.Data.Persistence;

namespace TemplateService.Application.TodoItems.Get
{
    public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, Result<List<TodoItem>>>
    {
        private readonly AppDbContext _db;

        public GetTodoItemsQueryHandler(AppDbContext db)
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
