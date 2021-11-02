using System;

namespace TemplateService.Data.Domain.Core
{
    public class AuditableEntity : IAuditableEntity
    {
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public interface IAuditableEntity
    {
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }
}
