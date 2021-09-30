using Messaging.MiniService.Enums;

namespace Messaging.MiniService
{
    public interface ITodoItem
    {
        int Id { get; set; }
        string Title { get; set; }
        bool Done { get; set; }
        PriorityLevel Priority { get; set; }
    }
}
