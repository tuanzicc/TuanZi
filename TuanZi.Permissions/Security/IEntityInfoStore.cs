using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.Core;
using TuanZi.Core.EntityInfos;


namespace TuanZi.Security
{
    [IgnoreDependency]
    public interface IEntityInfoStore<TEntityInfo, in TEntityInfoInputDto>
        where TEntityInfo : IEntityInfo, IEntity<Guid>
        where TEntityInfoInputDto : EntityInfoInputDtoBase
    {
        IQueryable<TEntityInfo> EntityInfos { get; }
        Task<bool> CheckEntityInfoExists(Expression<Func<TEntityInfo, bool>> predicate, Guid id = default(Guid));
        Task<OperationResult> UpdateEntityInfos(params TEntityInfoInputDto[] dtos);


    }
}
