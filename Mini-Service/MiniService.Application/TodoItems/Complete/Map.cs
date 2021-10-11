using Messaging.MiniService;
using Mapster;

namespace MiniService.Application.TodoItems.Complete
{
    public class Map : IRegister
    {
        public void Register(TypeAdapterConfig config) => config
            .ForType<CompleteTodoItemCmd, CompleteTodoItemCommand>()
            .Map(o => o.Id, o => o.TodoId);
    }
}
