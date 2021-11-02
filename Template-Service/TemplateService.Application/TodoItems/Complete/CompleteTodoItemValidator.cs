using FluentValidation;

namespace TemplateService.Application.TodoItems.Complete
{
    public class CompleteTodoItemValidator : AbstractValidator<CompleteTodoItemCommand>
    {
        public CompleteTodoItemValidator()
        {
            RuleFor(v => v.Id).NotEqual(0);
        }
    }
}
