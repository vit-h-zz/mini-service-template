using FluentResults;
using MediatR;
using MiniService.Data.Domain.Entities;

namespace MiniService.Application.TodoItems.Update
{
    public class UpdateTodoItemCommand : TodoItem, IRequest<Result> { }
}
