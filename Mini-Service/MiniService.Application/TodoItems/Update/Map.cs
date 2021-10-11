using Mapster;
using Messaging.MiniService;
using MiniService.Data.Domain.Entities;

namespace MiniService.Application.TodoItems.Update
{
    public class Map : IRegister
    {
        public void Register(TypeAdapterConfig config) => config
            .ForType<TodoItem, WorkDone>()
            .Map(o => o.TodoId, o => o.Id)
            .Map(o => o.Priority, o => (int)o.Priority);
    }
}
