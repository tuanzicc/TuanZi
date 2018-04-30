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
        {
            throw new System.Exception(auditDatas.ToJsonString());
        }

        public Task SetAuditDatasAsync(IEnumerable<AuditEntity> auditDatas, CancellationToken cancelToken = default(CancellationToken))
        {
            throw new System.Exception(auditDatas.ToJsonString());
            return Task.FromResult(0);
        }
    }
}