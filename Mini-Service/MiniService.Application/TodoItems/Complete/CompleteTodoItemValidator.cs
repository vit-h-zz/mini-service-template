using System;
using Messaging.MiniService.Enums;
using FluentValidation;

namespace MiniService.Application.TodoItems.Complete
{
    public class CompleteTodoItemValidator : AbstractValidator<CompleteTodoItemCommand>
    {
        public CompleteTodoItemValidator()
        {
            RuleFor(v => v.Id).NotEqual(0);
        }
    }
}
