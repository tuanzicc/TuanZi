

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;


namespace TuanZi.Security
{
    [IgnoreDependency]
    public interface IModuleStore<TModule, in TModuleInputDto, in TModuleKey>
        where TModule : ModuleBase<TModuleKey>, IEntity<TModuleKey>
        where TModuleInputDto : ModuleInputDtoBase<TModuleKey>
        where TModuleKey : struct, IEquatable<TModuleKey>
    {
        IQueryable<TModule> Modules { get; }
        
        Task<bool> CheckModuleExists(Expression<Func<TModule, bool>> predicate, TModuleKey id = default(TModuleKey));
        Task<OperationResult> CreateModule(TModuleInputDto dto);
        Task<OperationResult> UpdateModule(TModuleInputDto dto);
        Task<OperationResult> DeleteModule(TModuleKey id);

    }
}