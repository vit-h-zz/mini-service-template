using MiniService.Data.Domain.Entities;
using Mapster;

namespace MiniService.Application.TodoItems.GetFinishedWork
{
    public class Map : IRegister
    {
        public void Register(TypeAdapterConfig config) => config
            .ForType<TodoItem, WorkItem>()
            .Map(o => o.TodoId, o => o.Id)
            .Map(o => o.Priority, o => (int)o.Priority);
    }
}
