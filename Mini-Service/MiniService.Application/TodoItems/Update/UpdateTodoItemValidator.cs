using System;
using FluentValidation;
using Messaging.MiniService.Enums;

namespace MiniService.Application.TodoItems.Update
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
