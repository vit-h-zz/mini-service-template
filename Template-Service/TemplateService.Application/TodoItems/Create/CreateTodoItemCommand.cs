using Messaging.TemplateService.Enums;
using TemplateService.Data.Domain.Entities;
using FluentResults;
using MediatR;

namespace TemplateService.Application.TodoItems.Create
{
    public record CreateTodoItemCommand(
        string Title,
        PriorityLevel Priority)
        : IRequest<Result<TodoItem>>;
}
