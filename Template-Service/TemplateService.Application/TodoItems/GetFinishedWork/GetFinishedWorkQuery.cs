using System.Collections.Generic;
using FluentResults;
using MediatR;
using Messaging.TemplateService;

namespace TemplateService.Application.TodoItems.GetFinishedWork
{
    public record GetFinishedWorkQuery : IRequest<Result<List<WorkItem>>>
    {
    }
}
