using Messaging.MiniService;
using Messaging.MiniService.Enums;
using MiniService.Data.Domain.Core;

namespace MiniService.Data.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class TodoItem : AuditableEntity, ITodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool Done { get; set; }
        public PriorityLevel Priority { get; set; }
    }
}
