using System.Collections.Generic;
using FluentResults;
using MediatR;
using TemplateService.Data.Domain.Entities;

namespace TemplateService.Application.TodoItems.Get
{
    public record GetTodoItemsQuery : IRequest<Result<List<TodoItem>>>
    {
    }
}
