

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TuanZi.Dependency;


namespace TuanZi.Security
{
    [IgnoreDependency]
    public interface IModuleFunctionStore<TModuleFunction>
    {
        IQueryable<TModuleFunction> ModuleFunctions { get; }

        
        Task<bool> CheckModuleFunctionExists(Expression<Func<TModuleFunction, bool>> predicate, Guid id = default(Guid));

    }
}