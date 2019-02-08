using System;
using System.Collections.Concurrent;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.Entity.Transactions;
using TuanZi.Exceptions;
using TuanZi.Extensions;


namespace TuanZi.Entity
{
    [Dependency(ServiceLifetime.Scoped, TryAdd = true)]
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, IUnitOfWork> _connStringUnitOfWorks
            = new ConcurrentDictionary<string, IUnitOfWork>();

        private readonly ConcurrentDictionary<Type, IUnitOfWork> _entityTypeUnitOfWorks
            = new ConcurrentDictionary<Type, IUnitOfWork>();

        public UnitOfWorkManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public bool HasCommited
        {
            get { return _connStringUnitOfWorks.Values.All(m => m.HasCommited); }
        }

        public IUnitOfWork GetUnitOfWork<TEntity, TKey>() where TEntity : IEntity<TKey>
        {
            Type entityType = typeof(TEntity);
            return GetUnitOfWork(entityType);
        }

        public IUnitOfWork GetUnitOfWork(Type entityType)
        {
            if (!entityType.IsEntityType())
            {
                throw new TuanException($"Type '{entityType}' is not an entity type");
            }
            IUnitOfWork unitOfWork = _entityTypeUnitOfWorks.GetOrDefault(entityType);
            if (unitOfWork != null)
            {
                return unitOfWork;
            }
            IEntityConfigurationTypeFinder typeFinder = _serviceProvider.GetService<IEntityConfigurationTypeFinder>();
            Type dbContextType = typeFinder.GetDbContextTypeForEntity(entityType);
            if (dbContextType == null)
            {
                throw new TuanException($"The context type of the entity class '{entityType}' cannot be found");
            }
            TuanDbContextOptions dbContextOptions = GetDbContextResolveOptions(dbContextType);
            DbContextResolveOptions resolveOptions = new DbContextResolveOptions(dbContextOptions);
            unitOfWork = _connStringUnitOfWorks.GetOrDefault(resolveOptions.ConnectionString);
            if (unitOfWork != null)
            {
                return unitOfWork;
            }
            unitOfWork = ActivatorUtilities.CreateInstance<UnitOfWork>(_serviceProvider, resolveOptions);
            _entityTypeUnitOfWorks.TryAdd(entityType, unitOfWork);
            _connStringUnitOfWorks.TryAdd(resolveOptions.ConnectionString, unitOfWork);

            return unitOfWork;
        }

        public Type GetDbContextType(Type entityType)
        {
            IEntityConfigurationTypeFinder typeFinder = _serviceProvider.GetService<IEntityConfigurationTypeFinder>();
            return typeFinder.GetDbContextTypeForEntity(entityType);
        }

        public TuanDbContextOptions GetDbContextResolveOptions(Type dbContextType)
        {
            TuanDbContextOptions dbContextOptions = _serviceProvider.GetTuanOptions()?.GetDbContextOptions(dbContextType);
            if (dbContextOptions == null)
            {
                throw new TuanException($"Unable to find configuration information for data context '{dbContextType}'");
            }
            return dbContextOptions;
        }

        public void Commit()
        {
            foreach (IUnitOfWork unitOfWork in _connStringUnitOfWorks.Values)
            {
                unitOfWork.Commit();
            }
        }

        public void Dispose()
        {
            foreach (IUnitOfWork unitOfWork in _connStringUnitOfWorks.Values)
            {
                unitOfWork.Dispose();
            }

            _connStringUnitOfWorks.Clear();
        }
    }

}