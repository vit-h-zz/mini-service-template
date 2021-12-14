using VH.MiniService.Common.Application.Abstractions;
using FluentResults;
using MediatR;
using Messaging.TemplateService;

namespace TemplateService.Application.TodoItems.Complete
{
    public record CompleteTodoItemCommand(int Id) : IRequest<Result<CompleteTodoItemResult>>, IRequireUser { }
}
