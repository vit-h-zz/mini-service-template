using System.Collections.Generic;
using FluentResults;
using MediatR;

namespace MiniService.Application.TodoItems.GetFinishedWork
{
    public record GetFinishedWorkQuery : IRequest<Result<List<WorkItem>>>
    {
    }
}
