using FluentResults;
using MediatR;

namespace MiniService.Application.TodoItems.Delete
{
    public record DeleteTodoItemCommand(int Id) : IRequest<Result>;
}
