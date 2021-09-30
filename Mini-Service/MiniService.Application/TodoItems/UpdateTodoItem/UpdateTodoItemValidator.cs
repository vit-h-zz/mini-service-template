using System;
using Messaging.MiniService.Enums;
using FluentValidation;

namespace MiniService.Application.TodoItems.UpdateTodoItem
{
    public class UpdateTodoItemValidator : AbstractValidator<UpdateTodoItemCommand>
    {
        public UpdateTodoItemValidator()
        {
            RuleFor(v => v.Id).NotEqual(0);
            RuleFor(v => v.Priority).NotEqual(PriorityLevel.None);
            RuleFor(v => v.Title).MaximumLength(200).NotEmpty();
            RuleFor(v => v.Updated).NotEqual(default(DateTime));
        }
    }
}
