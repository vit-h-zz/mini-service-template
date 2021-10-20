using Messaging.Common;
using Messaging.Common.Enums;
using Messaging.MiniService.Enums;

namespace Messaging.MiniService
{
    public class TodoUpdated : IEvent<ITodoItem>
    {
        public TodoUpdated(ITodoItem todoItem, EventType eventType, string userId)
        {
            Data = todoItem;
            EventType = eventType;
            UserId = userId;
        }

        public EventType EventType { get; }
        public ITodoItem? Data { get; }
        public string UserId { get; }
    }

    public interface ITodoItem
    {
        int Id { get; set; }
        string Title { get; set; }
        bool Done { get; set; }
        PriorityLevel Priority { get; set; }
    }
}
