using Messaging.TemplateService;
using Messaging.TemplateService.Enums;
using TemplateService.Data.Domain.Core;

namespace TemplateService.Data.Domain.Entities
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
