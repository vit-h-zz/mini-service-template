using System.Collections.Generic;
using MiniService.Data.Domain.Entities;
using FluentResults;
using MediatR;

namespace MiniService.Application.TodoItems.GetTodoItems
{
    public record GetTodoItemsQuery : IRequest<Result<List<TodoItem>>>
    {
    }
}
