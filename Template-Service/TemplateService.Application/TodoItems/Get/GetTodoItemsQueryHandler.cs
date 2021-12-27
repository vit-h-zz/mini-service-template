using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TemplateService.Data.Domain.Entities;
using TemplateService.Data.Persistence;

namespace TemplateService.Application.TodoItems.Get
{
    public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, Result<List<TodoItem>>>
    {
        private readonly AppDbContext _db;
        private readonly IDistributedCache _cache;

        public GetTodoItemsQueryHandler(AppDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<Result<List<TodoItem>>> Handle(GetTodoItemsQuery r, CancellationToken ct)
        {
            const string Key = "testKey";
            var data = await _cache.GetStringAsync(Key, ct);
            await _cache.SetStringAsync(Key, data ?? "test data", ct);

            return Result.Ok(await _db.TodoItems
                .OrderBy(x => x.Title)
                .ToListAsync(ct));
        }
    }
}
