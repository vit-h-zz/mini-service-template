using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MiniService.Data.Persistence;
using FluentResults;
using Mapster;
using MediatR;
using Messaging.MiniService;
using Microsoft.EntityFrameworkCore;

namespace MiniService.Application.TodoItems.GetFinishedWork
{
    public class GetFinishedWorkHandler : IRequestHandler<GetFinishedWorkQuery, Result<List<WorkItem>>>
    {
        private readonly AppDbContext _db;

        public GetFinishedWorkHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Result<List<WorkItem>>> Handle(GetFinishedWorkQuery r, CancellationToken ct)
        {
            return Result.Ok(await _db.TodoItems
                .Where(x => x.Done)
                .OrderBy(x => x.Priority)
                .ProjectToType<WorkItem>()
                .ToListAsync(ct));
        }
    }
}
