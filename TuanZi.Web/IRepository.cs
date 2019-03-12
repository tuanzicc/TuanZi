using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using TuanZi.Data;
using TuanZi.Dependency;


namespace TuanZi.Entity
{
    public partial interface IRepository<TEntity, TKey>
         where TEntity : IEntity<TKey>
    {

        Task<int> UpdateAsync(IInputDto<TKey> dto);

        Task<int> InsertAsync(IInputDto<TKey> dto);

    }
}
