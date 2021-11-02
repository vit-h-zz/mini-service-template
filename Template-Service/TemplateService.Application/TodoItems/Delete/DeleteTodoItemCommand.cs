using FluentResults;
using MediatR;

namespace TemplateService.Application.TodoItems.Delete
{
    public record DeleteTodoItemCommand(int Id) : IRequest<Result>;
}
