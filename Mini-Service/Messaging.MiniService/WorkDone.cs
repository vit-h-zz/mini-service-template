using Messaging.Common;
using Messaging.Common.Enums;

namespace Messaging.MiniService
{
    public class WorkDone : IEvent<WorkItem>
    {
        public WorkDone(WorkItem item, string userId)
        {
            Data = item;
            UserId = userId;
        }

        public EventType EventType => EventType.Other;
        public string UserId { get; set; }
        public WorkItem? Data { get; set; }
    }

    public class WorkItem
    {
        public int TodoId { get; set; }
        public string? Title { get; set; }
        public int Priority { get; set; }
    }
}
