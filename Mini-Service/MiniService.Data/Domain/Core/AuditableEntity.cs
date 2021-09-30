using System;

namespace MiniService.Data.Domain.Core
{
    //Review me: why not to make it a base class AuditableEntity instead of interface IAuditableModel?
    public class AuditableEntity
    {
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
