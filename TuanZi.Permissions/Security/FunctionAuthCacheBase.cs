

using System;
using System.Linq;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Caching;
using TuanZi.Core.Functions;
using TuanZi.Entity;
using TuanZi.Identity;
using TuanZi.Secutiry;


namespace TuanZi.Security
{
    public abstract class FunctionAuthCacheBase<TModuleFunction, TModuleRole, TModuleUser, TFunction, TModule, TModuleKey,
        TRole, TRoleKey, TUser, TUserKey>
        : IFunctionAuthCache
        where TFunction : class, IFunction, IEntity<Guid>
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

        protected FunctionAuthCacheBase(IDistributedCache cache)
        {
            _cache = cache;
        }

        public virtual void BuildRoleCaches()
        {
            TFunction[] functions = ServiceLocator.Instance.ExcuteScopedWork(provider =>
            {
                IRepository<TFunction, Guid> functionRepository = provider.GetService<IRepository<TFunction, Guid>>();
                return functionRepository.Entities.ToArray();
            });

            foreach (TFunction function in functions)
            {
                GetFunctionRoles(function.Id);
            }
        }

        public virtual void RemoveFunctionCaches(params Guid[] functionIds)
        {
            foreach (Guid functionId in functionIds)
            {
                string key = $"Security_FunctionRoles_{functionId}";
                _cache.Remove(key);
            }
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
                return roleNames;
            }
            roleNames = ServiceLocator.Instance.ExcuteScopedWork(provider =>
            {
                IRepository<TModuleFunction, Guid> moduleFunctionRepository = provider.GetService<IRepository<TModuleFunction, Guid>>();
                TModuleKey[] moduleIds = moduleFunctionRepository.Entities.Where(m => m.FunctionId.Equals(functionId)).Select(m => m.ModuleId).Distinct()
                    .ToArray();
                IRepository<TModuleRole, Guid> moduleRoleRepository = provider.GetService<IRepository<TModuleRole, Guid>>();
                TRoleKey[] roleIds = moduleRoleRepository.Entities.Where(m => moduleIds.Contains(m.ModuleId)).Select(m => m.RoleId).Distinct().ToArray();
                IRepository<TRole, TRoleKey> roleRepository = provider.GetService<IRepository<TRole, TRoleKey>>();
                return roleRepository.Entities.Where(m => roleIds.Contains(m.Id)).Select(m => m.Name).Distinct().ToArray();
            });
            if (roleNames.Length > 0)
            {
                _cache.Set(key, roleNames);
            }
            return roleNames;
        }

        public virtual Guid[] GetUserFunctions(string userName)
        {
            string key = $"Security_UserFunctions_{userName}";
            Guid[] functionIds = _cache.Get<Guid[]>(key);
            if (functionIds != null)
            {
                return functionIds;
            }
            functionIds = ServiceLocator.Instance.ExcuteScopedWork(provider =>
            {
                IRepository<TUser, TUserKey> userRepository = provider.GetService<IRepository<TUser, TUserKey>>();
                TUserKey userId = userRepository.Entities.Where(m => m.UserName == userName).Select(m => m.Id).FirstOrDefault();
                if (Equals(userId, default(TUserKey)))
                {
                    return new Guid[0];
                }
                IRepository<TModuleUser, Guid> moduleUserRepository = provider.GetService<IRepository<TModuleUser, Guid>>();
                TModuleKey[] moduleIds = moduleUserRepository.Entities.Where(m => m.UserId.Equals(userId)).Select(m => m.ModuleId).Distinct().ToArray();
                IRepository<TModule, TModuleKey> moduleRepository = provider.GetService<IRepository<TModule, TModuleKey>>();
                moduleIds = moduleIds.Select(m => moduleRepository.Entities.Where(n => n.TreePathString.Contains("$" + m + "$"))
                    .Select(n => n.Id)).SelectMany(m => m).Distinct().ToArray();
                IRepository<TModuleFunction, Guid> moduleFunctionRepository = provider.GetService<IRepository<TModuleFunction, Guid>>();
                return moduleFunctionRepository.Entities.Where(m => moduleIds.Contains(m.ModuleId)).Select(m => m.FunctionId).Distinct().ToArray();
            });

            if (functionIds.Length > 0)
            {
                _cache.Set(key, functionIds);
            }
            return functionIds;
        }
    }
}