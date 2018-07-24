using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TuanZi.Data;
using TuanZi.Dependency;


namespace TuanZi.Security
{
    public interface IModuleRoleStore<TModuleRole, in TRoleKey, TModuleKey>
    {
        IQueryable<TModuleRole> ModuleRoles { get; }

        Task<bool> CheckModuleRoleExists(Expression<Func<TModuleRole, bool>> predicate, Guid id = default(Guid));

        Task<OperationResult> SetRoleModules(TRoleKey roleId, TModuleKey[] moduleIds);

        TModuleKey[] GetRoleModuleIds(TRoleKey roleId);

    }
}