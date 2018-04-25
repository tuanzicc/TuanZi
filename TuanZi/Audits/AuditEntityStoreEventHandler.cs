using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TuanZi.EventBuses;


namespace TuanZi.Audits
{
    public class AuditEntityStoreEventHandler : EventHandlerBase<AuditEntityEventData>
    {
        private readonly IAuditStore _auditStore;

        public AuditEntityStoreEventHandler(IAuditStore auditStore)
        {
            _auditStore = auditStore;
        }
        
        public override void Handle(AuditEntityEventData eventData)
        {
            eventData.CheckNotNull("eventData");
            _auditStore.SetAuditDatas(eventData.AuditEntities);
        }

        public override Task HandleAsync(AuditEntityEventData eventData, CancellationToken cancelToken = default(CancellationToken))
        {
            eventData.CheckNotNull("eventData" );
            cancelToken.ThrowIfCancellationRequested();
            return _auditStore.SetAuditDatasAsync(eventData.AuditEntities, cancelToken);
        }

    }
}
