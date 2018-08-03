using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TuanZi.Dependency;
using TuanZi.EventBuses;
using TuanZi.Extensions;


namespace TuanZi.Audits
{
    public class AuditEntityEventHandler : EventHandlerBase<AuditEntityEventData>
    {
        private readonly ScopedDictionary _scopedDictionary;

        public AuditEntityEventHandler(ScopedDictionary scopedDictionary)
        {
            _scopedDictionary = scopedDictionary;
        }

        public override void Handle(AuditEntityEventData eventData)
        {
            eventData.CheckNotNull("eventData");

            AuditOperationEntry operation = _scopedDictionary.AuditOperation;
            if (operation == null)
            {
                return;
            }
            foreach (AuditEntityEntry auditEntity in eventData.AuditEntities)
            {
                SetAddedId(auditEntity);
                operation.EntityEntries.Add(auditEntity);
            }
        }

        public override Task HandleAsync(AuditEntityEventData eventData, CancellationToken cancelToken = default(CancellationToken))
        {
            eventData.CheckNotNull("eventData");
            cancelToken.ThrowIfCancellationRequested();

            AuditOperationEntry operation = _scopedDictionary.AuditOperation;
            if (operation == null)
            {
                return Task.FromResult(0);
            }
            foreach (AuditEntityEntry auditEntity in eventData.AuditEntities)
            {
                SetAddedId(auditEntity);
                operation.EntityEntries.Add(auditEntity);
            }
            return Task.FromResult(0);
        }

        private static void SetAddedId(AuditEntityEntry entry)
        {
            if (entry.OperateType == OperateType.Insert)
            {
                dynamic entity = entry.Entity;
                entry.EntityKey = entity.Id.ToString();
                AuditPropertyEntry property = entry.PropertyEntries.FirstOrDefault(m => m.FieldName == "Id");
                if (property != null)
                {
                    property.NewValue = entity.Id.ToString();
                }
            }
        }
    }
}