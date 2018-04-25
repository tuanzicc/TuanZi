using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TuanZi.Dependency;


namespace TuanZi.Security
{
    [IgnoreDependency]
    public interface IModuleRoleStore<TModuleRole>
    {
        IQueryable<TModuleRole> ModuleRoles { get; }

        Task<bool> CheckModuleRoleExists(Expression<Func<TModuleRole, bool>> predicate, Guid id = default(Guid));

    }
}