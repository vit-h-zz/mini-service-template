using FluentResults;
using MediatR;

namespace MiniService.Application.TodoItems.DeleteTodoItem
{
    public record DeleteTodoItemCommand(int Id) : IRequest<Result>;
}
