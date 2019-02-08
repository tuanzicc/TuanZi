﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.Collections;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.Exceptions;
using TuanZi.Reflection;

namespace TuanZi.Core.EntityInfos
{
    public abstract class EntityInfoHandlerBase<TEntityInfo, TEntityInfoHandler> : IEntityInfoHandler
        where TEntityInfo : class, IEntityInfo, new()
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEntityTypeFinder _entityTypeFinder;
        private readonly ILogger _logger;
        private readonly List<TEntityInfo> _entityInfos = new List<TEntityInfo>();

        protected EntityInfoHandlerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _entityTypeFinder = serviceProvider.GetService<IEntityTypeFinder>();
            _logger = serviceProvider.GetLogger<TEntityInfoHandler>();
        }

        public void Initialize()
        {
            Type[] entityTypes = _entityTypeFinder.FindAll(true);
            _logger.LogInformation($"The data entity processor starts to initialize and finds {entityTypes.Length} entity classes.");
            foreach (Type entityType in entityTypes)
            {
                if (_entityInfos.Exists(m => m.TypeName == entityType.FullName))
                {
                    continue;
                }
                TEntityInfo entityInfo = new TEntityInfo();
                entityInfo.FromType(entityType);
                _entityInfos.Add(entityInfo);
            }

            _serviceProvider.ExecuteScopedWork(provider =>
            {
                SyncToDatabase(provider, _entityInfos);
            });

            RefreshCache();
        }

        public IEntityInfo GetEntityInfo(Type type)
        {
            Check.NotNull(type, nameof(type));
            if (_entityInfos.Count == 0)
            {
                RefreshCache();
            }
            string typeName = type.GetFullNameWithModule();
            IEntityInfo entityInfo = _entityInfos.FirstOrDefault(m => m.TypeName == typeName);
            if (entityInfo != null)
            {
                return entityInfo;
            }
            if (type.BaseType == null)
            {
                return null;
            }
            typeName = type.BaseType.GetFullNameWithModule();
            return _entityInfos.FirstOrDefault(m => m.TypeName == typeName);
        }

        public IEntityInfo GetEntityInfo<TEntity, TKey>() where TEntity : IEntity<TKey> where TKey : IEquatable<TKey>
        {
            Type type = typeof(TEntity);
            return GetEntityInfo(type);
        }

        public void RefreshCache()
        {
            _serviceProvider.ExecuteScopedWork(provider =>
            {
                _entityInfos.Clear();
                _entityInfos.AddRange(GetFromDatabase(provider));
            });
        }

        protected virtual void SyncToDatabase(IServiceProvider scopedProvider, List<TEntityInfo> entityInfos)
        {
            IRepository<TEntityInfo, Guid> repository = scopedProvider.GetService<IRepository<TEntityInfo, Guid>>();
            if (repository == null)
            {
                _logger.LogWarning("The service of IRepository<,> is not found, please initialize the Entity Pack module");
                return;
            }

            if (!entityInfos.CheckSyncByHash(scopedProvider, _logger))
            {
                _logger.LogInformation("The module data signature is the same as last time, synchronization has been cancelled");
                return;
            }

            TEntityInfo[] dbItems = repository.TrackQuery(null, false).ToArray();

            TEntityInfo[] removeItems = dbItems.Except(entityInfos, EqualityHelper<TEntityInfo>.CreateComparer(m => m.TypeName)).ToArray();
            int removeCount = removeItems.Length;
            repository.Delete(removeItems);

            TEntityInfo[] addItems = entityInfos.Except(dbItems, EqualityHelper<TEntityInfo>.CreateComparer(m => m.TypeName)).ToArray();
            int addCount = addItems.Length;
            repository.Insert(addItems);

            int updateCount = 0;
            foreach (TEntityInfo item in dbItems.Except(removeItems))
            {
                bool isUpdate = false;
                TEntityInfo entityInfo = entityInfos.SingleOrDefault(m => m.TypeName == item.TypeName);
                if (entityInfo == null)
                {
                    continue;
                }
                if (item.Name != entityInfo.Name)
                {
                    item.Name = entityInfo.Name;
                    isUpdate = true;
                }
                if (item.PropertyJson != entityInfo.PropertyJson)
                {
                    item.PropertyJson = entityInfo.PropertyJson;
                    isUpdate = true;
                }
                if (isUpdate)
                {
                    repository.Update(item);
                    updateCount++;
                }
            }
            repository.UnitOfWork.Commit();
            if (removeCount + addCount + updateCount > 0)
            {
                string msg = "Entity Changes";
                if (addCount > 0)
                {
                    msg += $", Added {addCount}";
                    _logger.LogInformation($"Added {removeItems.Length} Entities: {removeItems.Select(m => m.TypeName).ExpandAndToString()}");
                }
                if (removeCount > 0)
                {
                    msg += $", Deleted {removeCount}";
                    _logger.LogInformation($"Deleted {addItems.Length} Entities: {addItems.Select(m => m.TypeName).ExpandAndToString()}");
                }
                if (updateCount > 0)
                {
                    msg += $", Updated {updateCount}";
                    _logger.LogInformation($"Updated {updateCount} Entities");
                }
                _logger.LogInformation(msg);
            }
        }

        protected virtual TEntityInfo[] GetFromDatabase(IServiceProvider scopedProvider)
        {
            IRepository<TEntityInfo, Guid> repository = scopedProvider.GetService<IRepository<TEntityInfo, Guid>>();
            if (repository == null)
            {
                return new TEntityInfo[0];
            }
            return repository.Query(null, false).ToArray();
        }
    }


}