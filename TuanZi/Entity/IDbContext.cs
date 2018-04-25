using System.Threading;
using System.Threading.Tasks;


namespace TuanZi.Entity
{
    public interface IDbContext
    {
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancelToken = default(CancellationToken));
    }
}