using Messaging.MiniService.Enums;
using MiniService.Data.Domain.Entities;
using FluentResults;
using MediatR;

namespace MiniService.Application.TodoItems.CreateTodoItem
{
    public record CreateTodoItemCommand(
        string Title,
        PriorityLevel Priority)
        : IRequest<Result<TodoItem>>;
}
