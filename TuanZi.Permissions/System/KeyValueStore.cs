using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using TuanZi.Caching;
using TuanZi.Core.Data;
using TuanZi.Core.Systems;
using TuanZi.Data;
using TuanZi.Entity;


namespace TuanZi.Systems
{
    public class KeyValueStore : IKeyValueStore
    {
        private readonly IRepository<KeyValue, Guid> _keyValueRepository;
        private readonly IDistributedCache _cache;

        private const string AllKeyValuesKey = "All_KeyValue_Key";

        public KeyValueStore(IRepository<KeyValue, Guid> keyValueRepository,
            IDistributedCache cache)
        {
            _keyValueRepository = keyValueRepository;
            _cache = cache;
        }

        public IQueryable<KeyValue> KeyValues => _keyValueRepository.Query();

        public TSetting GetSetting<TSetting>() where TSetting : ISetting, new()
        {
            TSetting setting = new TSetting();
            Type type = typeof(TSetting);
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(m => m.PropertyType == typeof(IKeyValue)))
            {
                string key = ((KeyValue)property.GetValue(setting)).Key;
                IKeyValue keyValue = GetKeyValue(key);
                if (keyValue != null)
                {
                    property.SetValue(setting, keyValue);
                }
            }
            return setting;
        }

        public async Task<OperationResult> SaveSetting(ISetting setting)
        {
            Type type = setting.GetType();
            IKeyValue[] keyValues = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType == typeof(IKeyValue))
                .Select(p => (IKeyValue)p.GetValue(setting)).ToArray();
            return await CreateOrUpdateKeyValues(keyValues);
        }

        public IKeyValue GetKeyValue(string key)
        {
            const int seconds = 60 * 1000;
            KeyValue[] pairs = _cache.Get(AllKeyValuesKey, () => _keyValueRepository.Query().ToArray(), seconds);
            return pairs.FirstOrDefault(m => m.Key == key);
        }

        public Task<bool> CheckKeyValueExists(Expression<Func<KeyValue, bool>> predicate, Guid id = default(Guid))
        {
            return _keyValueRepository.CheckExistsAsync(predicate, id);
        }

        public Task<OperationResult> CreateOrUpdateKeyValue(string key, object value)
        {
            IKeyValue pair = new KeyValue(key, value);
            return CreateOrUpdateKeyValues(pair);
        }

        public async Task<OperationResult> CreateOrUpdateKeyValues(params IKeyValue[] dtos)
        {
            Check.NotNull(dtos, nameof(dtos));
            int count = 0;
            foreach (IKeyValue dto in dtos)
            {
                KeyValue pair = _keyValueRepository.TrackQuery().FirstOrDefault(m => m.Key == dto.Key);
                if (pair == null)
                {
                    pair = new KeyValue(dto.Key, dto.Value);
                    count += await _keyValueRepository.InsertAsync(pair);
                }
                else if (pair.Value != dto.Value)
                {
                    pair.Value = dto.Value;
                    count += await _keyValueRepository.UpdateAsync(pair);
                }
            }
            if (count > 0)
            {
                await _cache.RemoveAsync(AllKeyValuesKey);
            }
            return OperationResult.Success;
        }

        public async Task<OperationResult> DeleteKeyValues(params Guid[] ids)
        {
            OperationResult result = await _keyValueRepository.DeleteAsync(ids);
            if (result.Successed)
            {
                await _cache.RemoveAsync(AllKeyValuesKey);
            }
            return result;
        }

        public async Task<OperationResult> DeleteKeyValues(string rootKey)
        {
            Guid[] ids = _keyValueRepository.Query(m => m.Key.StartsWith(rootKey)).Select(m => m.Id).ToArray();
            return await DeleteKeyValues(ids);
        }
    }
}