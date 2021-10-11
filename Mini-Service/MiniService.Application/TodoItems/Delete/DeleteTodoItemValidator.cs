using FluentValidation;

namespace MiniService.Application.TodoItems.Delete
{
    public class DeleteTodoItemValidator : AbstractValidator<DeleteTodoItemCommand>
    {
        public DeleteTodoItemValidator()
        {
            RuleFor(v => v.Id).NotEqual(0);
        }
    }
}
