using Messaging.MiniService.Enums;

namespace Messaging.MiniService
{
    public class TodoUpdated
    {
        public TodoUpdated(ITodoItem todoItem, DataChangeType changeType)
        {
            TodoItem = todoItem;
            ChangeType = changeType;
        }

        public ITodoItem TodoItem { get; set; }
        public DataChangeType ChangeType { get; set; }
    }
}
