using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace TuanZi.Audits
{
    public interface IAuditStore
    {
        void SetAuditDatas(IEnumerable<AuditEntity> auditDatas);

        Task SetAuditDatasAsync(IEnumerable<AuditEntity>auditDatas, CancellationToken cancelToken = default(CancellationToken));
    }


    public class NullAuditStore : IAuditStore
    {
        public void SetAuditDatas(IEnumerable<AuditEntity> auditDatas)
        { }

        public Task SetAuditDatasAsync(IEnumerable<AuditEntity> auditDatas, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }
    }
}