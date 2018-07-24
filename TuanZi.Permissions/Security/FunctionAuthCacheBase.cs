

using System;
using System.Linq;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.Caching;
using TuanZi.Core.Functions;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.Identity;
using TuanZi.Secutiry;


namespace TuanZi.Security
{
    public abstract class FunctionAuthCacheBase<TModuleFunction, TModuleRole, TModuleUser, TFunction, TModule, TModuleKey,
       TRole, TRoleKey, TUser, TUserKey>
       : IFunctionAuthCache
       where TFunction : class, IFunction
       where TModule : ModuleBase<TModuleKey>
       where TModuleFunction : ModuleFunctionBase<TModuleKey>
       where TModuleKey : struct, IEquatable<TModuleKey>
       where TModuleRole : ModuleRoleBase<TModuleKey, TRoleKey>
       where TModuleUser : ModuleUserBase<TModuleKey, TUserKey>
       where TRole : RoleBase<TRoleKey>
       where TRoleKey : IEquatable<TRoleKey>
       where TUser : UserBase<TUserKey>
       where TUserKey : IEquatable<TUserKey>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        protected FunctionAuthCacheBase(IDistributedCache cache)
        {
            _cache = cache;
            _logger = ServiceLocator.Instance.GetLogger(GetType());
        }

        public virtual void BuildRoleCaches()
        {
            TFunction[] functions = ServiceLocator.Instance.ExcuteScopedWork(provider =>
            {
                IRepository<TFunction, Guid> functionRepository = provider.GetService<IRepository<TFunction, Guid>>();
                return functionRepository.Query().ToArray();
            });

            foreach (TFunction function in functions)
            {
                GetFunctionRoles(function.Id);
            }
            _logger.LogInformation($"Create a 'Function-Roles[]' cache for {functions.Length} functions");
        }

        public virtual void RemoveFunctionCaches(params Guid[] functionIds)
        {
            foreach (Guid functionId in functionIds)
            {
                string key = $"Security_FunctionRoles_{functionId}";
                _cache.Remove(key);
                _logger.LogDebug($"Remove the 'Function-Roles[]' cache of the function '{functionId}'");
            }
            _logger.LogInformation($"Remove {functionIds.Length} 'Function-Roles[]' cache");
        }

        public virtual void RemoveUserCaches(params string[] userNames)
        {
            foreach (string userName in userNames)
            {
                string key = $"Security_UserFunctions_{userName}";
                _cache.Remove(key);
            }
        }

        public virtual string[] GetFunctionRoles(Guid functionId)
        {
            string key = $"Security_FunctionRoles_{functionId}";
            string[] roleNames = _cache.Get<string[]>(key);
            if (roleNames != null)
            {
                _logger.LogDebug($"Get the 'Function-Roles[]' cache of the function '{functionId}' from the cache");
                return roleNames;
            }
            roleNames = ServiceLocator.Instance.ExcuteScopedWork(provider =>
            {
                IRepository<TModuleFunction, Guid> moduleFunctionRepository = provider.GetService<IRepository<TModuleFunction, Guid>>();
                TModuleKey[] moduleIds = moduleFunctionRepository.Query().Where(m => m.FunctionId.Equals(functionId)).Select(m => m.ModuleId).Distinct()
                    .ToArray();
                IRepository<TModuleRole, Guid> moduleRoleRepository = provider.GetService<IRepository<TModuleRole, Guid>>();
                TRoleKey[] roleIds = moduleRoleRepository.Query().Where(m => moduleIds.Contains(m.ModuleId)).Select(m => m.RoleId).Distinct().ToArray();
                IRepository<TRole, TRoleKey> roleRepository = provider.GetService<IRepository<TRole, TRoleKey>>();
                return roleRepository.Query().Where(m => roleIds.Contains(m.Id)).Select(m => m.Name).Distinct().ToArray();
            });
            if (roleNames.Length > 0)
            {
                _cache.Set(key, roleNames);
                _logger.LogDebug($"Add the 'Function-Roles[]' cache of the function '{functionId}'");
            }
            return roleNames;
        }

        public virtual Guid[] GetUserFunctions(string userName)
        {
            string key = $"Security_UserFunctions_{userName}";
            Guid[] functionIds = _cache.Get<Guid[]>(key);
            if (functionIds != null)
            {
                _logger.LogDebug($"Get the 'User-Function[]' cache of the user '{userName}' from the cache");
                return functionIds;
            }
            functionIds = ServiceLocator.Instance.ExcuteScopedWork(provider =>
            {
                IRepository<TUser, TUserKey> userRepository = provider.GetService<IRepository<TUser, TUserKey>>();
                TUserKey userId = userRepository.Query().Where(m => m.UserName == userName).Select(m => m.Id).FirstOrDefault();
                if (Equals(userId, default(TUserKey)))
                {
                    return new Guid[0];
                }
                IRepository<TModuleUser, Guid> moduleUserRepository = provider.GetService<IRepository<TModuleUser, Guid>>();
                TModuleKey[] moduleIds = moduleUserRepository.Query().Where(m => m.UserId.Equals(userId)).Select(m => m.ModuleId).Distinct().ToArray();
                IRepository<TModule, TModuleKey> moduleRepository = provider.GetService<IRepository<TModule, TModuleKey>>();
                moduleIds = moduleIds.Select(m => moduleRepository.Query().Where(n => n.TreePathString.Contains("$" + m + "$"))
                    .Select(n => n.Id)).SelectMany(m => m).Distinct().ToArray();
                IRepository<TModuleFunction, Guid> moduleFunctionRepository = provider.GetService<IRepository<TModuleFunction, Guid>>();
                return moduleFunctionRepository.Query().Where(m => moduleIds.Contains(m.ModuleId)).Select(m => m.FunctionId).Distinct().ToArray();
            });

            if (functionIds.Length > 0)
            {
                _logger.LogDebug($"Create the 'User-Function[]' cache for user '{userName}'");
                _cache.Set(key, functionIds);
            }
            return functionIds;
        }
    }
}