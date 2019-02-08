using System;
using System.Collections.Concurrent;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Dependency;
using TuanZi.Extensions;


namespace TuanZi.Entity
{
    [Dependency(ServiceLifetime.Singleton, TryAdd = true, AddSelf = true)]
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