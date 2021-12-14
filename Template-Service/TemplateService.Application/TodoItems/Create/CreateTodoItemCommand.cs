using Messaging.TemplateService.Enums;
using TemplateService.Data.Domain.Entities;
using FluentResults;
using MediatR;
using VH.MiniService.Common.Application.Abstractions;

namespace TemplateService.Application.TodoItems.Create
{
    public record CreateTodoItemCommand(
        string Title,
        PriorityLevel Priority)
        : IRequest<Result<TodoItem>>, IRequireUser;
}
