using System.Collections.Generic;

using TuanZi.Data;
using TuanZi.EventBuses;


namespace TuanZi.Audits
{
    public class AuditEntityEventData : EventDataBase
    {
        public AuditEntityEventData(IList<AuditEntityEntry> auditEntities)
        {
            Check.NotNull(auditEntities, nameof(auditEntities));

            AuditEntities = auditEntities;
        }

        public IEnumerable<AuditEntityEntry> AuditEntities { get; }
    }
}