using System.Collections.Generic;
using FluentResults;
using MediatR;
using Messaging.MiniService;

namespace MiniService.Application.TodoItems.GetFinishedWork
{
    public record GetFinishedWorkQuery : IRequest<Result<List<WorkItem>>>
    {
    }
}
