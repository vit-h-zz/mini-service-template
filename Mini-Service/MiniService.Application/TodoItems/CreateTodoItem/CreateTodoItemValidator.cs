using Messaging.MiniService.Enums;
using FluentValidation;

namespace MiniService.Application.TodoItems.CreateTodoItem
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
