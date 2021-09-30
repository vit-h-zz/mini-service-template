using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MiniService.Application.TodoItems.DeleteTodoItem
{
    public class DeleteTodoItemValidator : AbstractValidator<DeleteTodoItemCommand>
    {
        public DeleteTodoItemValidator()
        {
            RuleFor(v => v.Id).NotEqual(0);
        }
    }
}
