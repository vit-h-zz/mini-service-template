using VH.MiniService.Common.Application.Abstractions;
using FluentResults;
using FluentValidation;
using MediatR;
using Messaging.TemplateService.Enums;

namespace TemplateService.Application.TodoItems.Create
{
    public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemCommand>
    {
        public CreateTodoItemValidator()
        {
            RuleFor(v => v.Title).MaximumLength(200).NotEmpty();
            RuleFor(v => v.Priority).NotEqual(PriorityLevel.None);
        }
    }
}
