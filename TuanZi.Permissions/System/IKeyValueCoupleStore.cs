using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TuanZi.Data;


namespace TuanZi.System
{
    public interface IKeyValueCoupleStore
    {
        IQueryable<KeyValueCouple> KeyValueCouples { get; }

        KeyValueCouple GetKeyValueCouple(string key);

        Task<bool> CheckKeyValueCoupleExists(Expression<Func<KeyValueCouple, bool>> predicate, Guid id = default(Guid));

        Task<OperationResult> CreateOrUpdateKeyValueCouple(string key, object value);

        Task<OperationResult> CreateOrUpdateKeyValueCouples(params KeyValueCouple[] dtos);

        Task<OperationResult> DeleteKeyValueCouples(params Guid[] ids);

        Task<OperationResult> DeleteKeyValueCouples(string rootKey);
    }
}