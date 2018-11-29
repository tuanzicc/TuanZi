using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TuanZi.Core.Data;
using TuanZi.Data;


namespace TuanZi.Core.Systems
{
    public interface IKeyValueStore
    {
        IQueryable<KeyValue> KeyValues { get; }
        TSetting GetSetting<TSetting>() where TSetting : ISetting, new();
        Task<OperationResult> SaveSetting(ISetting setting);
        IKeyValue GetKeyValue(string key);

        Task<bool> CheckKeyValueExists(Expression<Func<KeyValue, bool>> predicate, Guid id = default(Guid));

        Task<OperationResult> CreateOrUpdateKeyValue(string key, object value);

        Task<OperationResult> CreateOrUpdateKeyValues(params IKeyValue[] dtos);

        Task<OperationResult> DeleteKeyValues(params Guid[] ids);

        Task<OperationResult> DeleteKeyValues(string rootKey);
    }
}