using System;
using System.Linq;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.Caching;
using TuanZi.Core.EntityInfos;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.Extensions;
using TuanZi.Filter;
using TuanZi.Identity;
using TuanZi.Secutiry;


namespace TuanZi.Security
{
    public abstract class DataAuthCacheBase<TEntityRole, TRole, TEntityInfo, TRoleKey> : IDataAuthCache
        where TEntityRole : EntityRoleBase<TRoleKey>
        where TRole : RoleBase<TRoleKey>
        where TEntityInfo : class, IEntityInfo
        where TRoleKey : IEquatable<TRoleKey>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        protected DataAuthCacheBase()
        {
            _cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            _logger = ServiceLocator.Instance.GetLogger(GetType());
        }

        public void BuildCaches()
        {
            var entityRoles = ServiceLocator.Instance.ExcuteScopedWork(provider =>
            {
                IRepository<TEntityRole, Guid> entityRoleRepository = provider.GetService<IRepository<TEntityRole, Guid>>();
                IRepository<TRole, TRoleKey> roleRepository = provider.GetService<IRepository<TRole, TRoleKey>>();
                IRepository<TEntityInfo, Guid> entityInfoRepository = provider.GetService<IRepository<TEntityInfo, Guid>>();
                return entityRoleRepository.Query(m => !m.IsLocked).Select(m => new
                {
                    m.FilterGroupJson,
                    m.Operation,
                    RoleName = roleRepository.Query(null, false).First(n => n.Id.Equals(m.RoleId)).Name,
                    EntityTypeFullName = entityInfoRepository.Query(null, false).First(n => n.Id == m.EntityId).TypeName
                }).ToArray();
            });

            foreach (var entityRole in entityRoles)
            {
                FilterGroup filterGroup = entityRole.FilterGroupJson.FromJsonString<FilterGroup>();
                string key = GetKey(entityRole.RoleName, entityRole.EntityTypeFullName, entityRole.Operation);
                string name = GetName(entityRole.RoleName, entityRole.EntityTypeFullName, entityRole.Operation);

                _cache.Set(key, filterGroup);
                _logger.LogDebug($"Create a data permission rule cache for {name}");
            }
            _logger.LogInformation($"Data permissions: create {entityRoles.Length} data permission filter rule cache");
        }

        public void SetCache(DataAuthCacheItem item)
        {
            string key = GetKey(item.RoleName, item.EntityTypeFullName, item.Operation);
            string name = GetName(item.RoleName, item.EntityTypeFullName, item.Operation);

            _cache.Set(key, item.FilterGroup);
            _logger.LogDebug($"Create a data permission rule cache for {name}");
        }

        public void RemoveCache(DataAuthCacheItem item)
        {
            string key = GetKey(item.RoleName, item.EntityTypeFullName, item.Operation);
            string name = GetName(item.RoleName, item.EntityTypeFullName, item.Operation);
            _cache.Remove(key);
            _logger.LogDebug($"Remove data permission rule cache for {name}");
        }

        public FilterGroup GetFilterGroup(string roleName, string entityTypeFullName, DataAuthOperation operation)
        {
            string key = GetKey(roleName, entityTypeFullName, operation);
            return _cache.Get<FilterGroup>(key);
        }

        private static string GetKey(string roleName, string entityTypeFullName, DataAuthOperation operation)
        {
            return $"Security_EntityRole_{roleName}_{entityTypeFullName}_{operation}";
        }

        private static string GetName(string roleName, string entityTypeFullName, DataAuthOperation operation)
        {
            return $"Role '{roleName}' and entity '{entityTypeFullName}' and operation '{operation}'";
        }
    }
}