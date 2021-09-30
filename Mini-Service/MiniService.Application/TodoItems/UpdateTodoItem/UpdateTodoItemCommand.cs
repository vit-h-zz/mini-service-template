using MiniService.Data.Domain.Entities;
using FluentResults;
using MediatR;

namespace MiniService.Application.TodoItems.UpdateTodoItem
{
    public class UpdateTodoItemCommand : TodoItem, IRequest<Result> { }
}
