using System.Collections.Generic;
using FluentResults;
using MediatR;
using MiniService.Data.Domain.Entities;

namespace MiniService.Application.TodoItems.Get
{
    public record GetTodoItemsQuery : IRequest<Result<List<TodoItem>>>
    {
    }
}
