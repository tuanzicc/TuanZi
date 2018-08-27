using System;
using System.Collections.Concurrent;

using Microsoft.EntityFrameworkCore.Metadata;

using TuanZi.Extensions;


namespace TuanZi.Entity
{
    public class DbContextModelCache
    {
        private readonly ConcurrentDictionary<Type, IModel> _dict = new ConcurrentDictionary<Type, IModel>();

        public IModel Get(Type dbContextType)
        {
            return _dict.GetOrDefault(dbContextType);
        }

        public void Set(Type dbContextType, IModel model)
        {
            _dict[dbContextType] = model;
        }

        public void Remove(Type dbContextType)
        {
            _dict.TryRemove(dbContextType, out IModel model);
        }
    }
}