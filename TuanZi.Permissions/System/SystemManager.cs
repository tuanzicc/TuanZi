using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using TuanZi.Caching;
using TuanZi.Data;
using TuanZi.Entity;


namespace TuanZi.System
{
    public class SystemManager : IKeyValueCoupleStore
    {
        private readonly IRepository<KeyValueCouple, Guid> _keyValueRepository;
        private readonly IDistributedCache _cache;

        public SystemManager(IRepository<KeyValueCouple, Guid> keyValueRepository,
            IDistributedCache cache)
        {
            _keyValueRepository = keyValueRepository;
            _cache = cache;
        }

        #region Implementation of IKeyValueCoupleStore

        private const string AllKeyValueCouplesKey = "All_KeyValueCouple_Key";

        public IQueryable<KeyValueCouple> KeyValueCouples
        {
            get { return _keyValueRepository.Entities; }
        }

        public KeyValueCouple GetKeyValueCouple(string key)
        {
            const int seconds = 60 * 1000;
            KeyValueCouple[] pairs = _cache.Get(AllKeyValueCouplesKey, () => _keyValueRepository.Entities.ToArray(), seconds);
            return pairs.FirstOrDefault(m => m.Key == key);
        }

        public Task<bool> CheckKeyValueCoupleExists(Expression<Func<KeyValueCouple, bool>> predicate, Guid id = default(Guid))
        {
            return _keyValueRepository.CheckExistsAsync(predicate, id);
        }

        public Task<OperationResult> CreateOrUpdateKeyValueCouple(string key, object value)
        {
            KeyValueCouple pair = new KeyValueCouple(key, value);
            return CreateOrUpdateKeyValueCouples(pair);
        }

        public async Task<OperationResult> CreateOrUpdateKeyValueCouples(params KeyValueCouple[] dtos)
        {
            Check.NotNull(dtos, nameof(dtos));
            foreach (KeyValueCouple dto in dtos)
            {
                KeyValueCouple pair = _keyValueRepository.TrackEntities.FirstOrDefault(m => m.Key == dto.Key);
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
            await _cache.RemoveAsync(AllKeyValueCouplesKey);
            return OperationResult.Success;
        }

        public async Task<OperationResult> DeleteKeyValueCouples(params Guid[] ids)
        {
            OperationResult result = await _keyValueRepository.DeleteAsync(ids);
            if (result.Successed)
            {
                await _cache.RemoveAsync(AllKeyValueCouplesKey);
            }
            return result;
        }

        public async Task<OperationResult> DeleteKeyValueCouples(string rootKey)
        {
            Guid[] ids = _keyValueRepository.Entities.Where(m => m.Key.StartsWith(rootKey)).Select(m => m.Id).ToArray();
            return await DeleteKeyValueCouples(ids);
        }

        #endregion
    }
}