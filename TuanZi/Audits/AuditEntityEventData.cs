using System.Collections.Generic;

using TuanZi.EventBuses;


namespace TuanZi.Audits
{
    public class AuditEntityEventData : EventDataBase
    {
        public AuditEntityEventData(IList<AuditEntity> auditEntities)
        {
            Check.NotNull(auditEntities, nameof(auditEntities));

            AuditEntities = auditEntities;
        }

        public IEnumerable<AuditEntity> AuditEntities { get; }
    }
}