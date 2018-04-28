

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TuanZi.Data;
using TuanZi.Dependency;


namespace TuanZi.Security
{
    [IgnoreDependency]
    public interface IModuleFunctionStore<TModuleFunction, in TModuleKey>
    {
        IQueryable<TModuleFunction> ModuleFunctions { get; }

        Task<bool> CheckModuleFunctionExists(Expression<Func<TModuleFunction, bool>> predicate, Guid id = default(Guid));

        Task<OperationResult> SetModuleFunctions(TModuleKey moduleId, Guid[] functionIds);
        
    }
}