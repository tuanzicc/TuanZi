using System.Threading;
using System.Threading.Tasks;


namespace TuanZi.Audits
{
    public interface IAuditStore
    {
        void Save(AuditOperationEntry operationEntry);

        Task SaveAsync(AuditOperationEntry operationEntry, CancellationToken cancelToken = default(CancellationToken));
    }
}