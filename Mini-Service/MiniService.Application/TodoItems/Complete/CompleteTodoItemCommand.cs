using FluentResults;
using MediatR;
using Messaging.MiniService;

namespace MiniService.Application.TodoItems.Complete
{
    public record CompleteTodoItemCommand(int Id) : IRequest<Result<CompleteTodoItemResult>> { }
}
