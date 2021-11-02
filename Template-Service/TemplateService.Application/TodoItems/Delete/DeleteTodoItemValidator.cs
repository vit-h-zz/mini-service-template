using FluentValidation;

namespace TemplateService.Application.TodoItems.Delete
{
    public class DeleteTodoItemValidator : AbstractValidator<DeleteTodoItemCommand>
    {
        public DeleteTodoItemValidator()
        {
            RuleFor(v => v.Id).NotEqual(0);
        }
    }
}
