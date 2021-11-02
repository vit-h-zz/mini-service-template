using FluentResults;
using MediatR;
using Messaging.TemplateService;

namespace TemplateService.Application.TodoItems.Complete
{
    public record CompleteTodoItemCommand(int Id) : IRequest<Result<CompleteTodoItemResult>> { }
}
