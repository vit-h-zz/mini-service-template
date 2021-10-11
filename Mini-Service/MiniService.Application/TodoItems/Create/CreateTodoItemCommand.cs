using Messaging.MiniService.Enums;
using MiniService.Data.Domain.Entities;
using FluentResults;
using MediatR;

namespace MiniService.Application.TodoItems.Create
{
    public record CreateTodoItemCommand(
        string Title,
        PriorityLevel Priority)
        : IRequest<Result<TodoItem>>;
}
