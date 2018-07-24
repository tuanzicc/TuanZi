using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TuanZi.Data;
using TuanZi.Filter;
using TuanZi.Secutiry;

namespace TuanZi.Security
{
    public interface IEntityRoleStore<TEntityRole, in TEntityRoleInputDto, in TRoleKey>
    {

        IQueryable<TEntityRole> EntityRoles { get; }

        Task<bool> CheckEntityRoleExists(Expression<Func<TEntityRole, bool>> predicate, Guid id = default(Guid));

        FilterGroup[] GetEntityRoleFilterGroups(TRoleKey roleId, Guid entityId, DataAuthOperation operation);

        Task<OperationResult> CreateEntityRoles(params TEntityRoleInputDto[] dtos);

        Task<OperationResult> UpdateEntityRoles(params TEntityRoleInputDto[] dtos);

        Task<OperationResult> DeleteEntityRoles(params Guid[] ids);


    }
}