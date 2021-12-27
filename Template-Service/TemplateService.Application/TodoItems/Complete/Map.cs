using Messaging.TemplateService;
using Mapster;
using TemplateService.Data.Domain.Entities;

namespace TemplateService.Application.TodoItems.Complete
{
    public class Map : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .ForType<CompleteTodoItemCmd, CompleteTodoItemCommand>()
                .Map(o => o.Id, o => o.TodoId);

            config
                .ForType<TodoItem, WorkItem>()
                .Map(o => o.TodoId, o => o.Id)
                .Map(o => o.Priority, o => (int)o.Priority);
        }
    }
}
