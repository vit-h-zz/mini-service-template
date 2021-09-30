using Messaging.MiniService;
using MiniService.Data.Domain.Entities;
using Mapster;

namespace MiniService.Application.TodoItems.UpdateTodoItem
{
    public class Map : IRegister
    {
        public void Register(TypeAdapterConfig config) => config
            .ForType<TodoItem, WorkDone>()
            .Map(o => o.TodoId, o => o.Id)
            .Map(o => o.Priority, o => (int)o.Priority);
    }
}
