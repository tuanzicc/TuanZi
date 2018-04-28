

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TuanZi.Data;
using TuanZi.Dependency;


namespace TuanZi.Security
{

    [IgnoreDependency]
    public interface IModuleUserStore<TModuleUser, in TUserKey, TModuleKey>
    {
        IQueryable<TModuleUser> ModuleUsers { get; }

        Task<bool> CheckModuleUserExists(Expression<Func<TModuleUser, bool>> predicate, Guid id = default(Guid));

        Task<OperationResult> SetUserModules(TUserKey userId, TModuleKey[] moduleIds);

        TModuleKey[] GetUserSelfModuleIds(TUserKey userId);

        TModuleKey[] GetUserWithRoleModuleIds(TUserKey userId);
        
    }
}