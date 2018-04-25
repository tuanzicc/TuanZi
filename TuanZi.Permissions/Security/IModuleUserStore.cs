

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TuanZi.Dependency;


namespace TuanZi.Security
{
    
    [IgnoreDependency]
    public interface IModuleUserStore<TModuleUser>
    {
        IQueryable<TModuleUser> ModuleUsers { get; }
        
        Task<bool> CheckModuleUserExists(Expression<Func<TModuleUser, bool>> predicate, Guid id = default(Guid));

     
    }
}