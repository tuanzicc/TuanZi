using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using TuanZi.Caching;
using TuanZi.Core.Systems;
using TuanZi.Data;
using TuanZi.Entity;


namespace TuanZi.Systems
{

    public class SystemManager : IKeyValueStore
    {
        private readonly IRepository<KeyValue, Guid> _keyValueRepository;
        private readonly IDistributedCache _cache;

        public SystemManager(IRepository<KeyValue, Guid> keyValueRepository,
            IDistributedCache cache)
        {
            _keyValueRepository = keyValueRepository;
            _cache = cache;
        }

        #region Implementation of IKeyValueCoupleStore

        private const string AllKeyValuesKey = "All_KeyValue_Key";

        public IQueryable<KeyValue> KeyValues
        {
            get { return _keyValueRepository.Query(); }
        }

        public KeyValue GetKeyValue(string key)
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
            KeyValue pair = new KeyValue(key, value);
            return CreateOrUpdateKeyValues(pair);
        }

        public async Task<OperationResult> CreateOrUpdateKeyValues(params KeyValue[] dtos)
        {
            Check.NotNull(dtos, nameof(dtos));
            foreach (KeyValue dto in dtos)
            {
                KeyValue pair = _keyValueRepository.TrackQuery().FirstOrDefault(m => m.Key == dto.Key);
                if (pair == null)
                {
                    pair = dto;
                    await _keyValueRepository.InsertAsync(pair);
                }
                else
                {
                    pair.Value = dto.Value;
                    await _keyValueRepository.UpdateAsync(pair);
                }
            }
            await _cache.RemoveAsync(AllKeyValuesKey);
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

        #endregion
    }
}