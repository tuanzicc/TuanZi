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
        private readonly IServiceProvider _serviceProvider;

        protected FunctionAuthCacheBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cache = serviceProvider.GetService<IDistributedCache>();
        }

        public virtual void BuildCaches()
        {
            TFunction[] functions;
            using (var scope = _serviceProvider.CreateScope())
            {
                IServiceProvider scopedProvider = scope.ServiceProvider;
                IRepository<TFunction, Guid> functionRepository = scopedProvider.GetService<IRepository<TFunction, Guid>>();
                functions = functionRepository.Query().ToArray();
            }
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
            if (roleNames == null)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    IServiceProvider scopedProvider = scope.ServiceProvider;
                    IRepository<TModuleFunction, Guid> moduleFunctionRepository = scopedProvider.GetService<IRepository<TModuleFunction, Guid>>();
                    TModuleKey[] moduleIds = moduleFunctionRepository.Query(m => m.FunctionId.Equals(functionId)).Select(m => m.ModuleId).Distinct()
                        .ToArray();
                    IRepository<TModuleRole, Guid> moduleRoleRepository = scopedProvider.GetService<IRepository<TModuleRole, Guid>>();
                    TRoleKey[] roleIds = moduleRoleRepository.Query(m => moduleIds.Contains(m.ModuleId)).Select(m => m.RoleId).Distinct().ToArray();
                    IRepository<TRole, TRoleKey> roleRepository = scopedProvider.GetService<IRepository<TRole, TRoleKey>>();
                    roleNames = roleRepository.Query(m => roleIds.Contains(m.Id)).Select(m => m.Name).Distinct().ToArray();
                }
                if (roleNames.Length > 0)
                {
                    _cache.Set(key, roleNames);
                }
            }
            return roleNames;
        }

        public virtual Guid[] GetUserFunctions(string userName)
        {
            string key = $"Security_UserFunctions_{userName}";
            Guid[] functionIds = _cache.Get<Guid[]>(key);
            if (functionIds == null)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    IServiceProvider scopedProvider = scope.ServiceProvider;
                    IRepository<TUser, TUserKey> userRepository = scopedProvider.GetService<IRepository<TUser, TUserKey>>();
                    TUserKey userId = userRepository.Query(m => m.UserName == userName).Select(m => m.Id).FirstOrDefault();
                    if (userId.Equals(default(TUserKey)))
                    {
                        return new Guid[0];
                    }
                    IRepository<TModuleUser, Guid> moduleUserRepository = scopedProvider.GetService<IRepository<TModuleUser, Guid>>();
                    TModuleKey[] moduleIds = moduleUserRepository.Query(m => m.UserId.Equals(userId)).Select(m => m.ModuleId).Distinct().ToArray();
                    IRepository<TModule, TModuleKey> moduleRepository = scopedProvider.GetService<IRepository<TModule, TModuleKey>>();
                    moduleIds = moduleIds.Select(m => moduleRepository.Query(n => n.TreePathString.Contains("$" + m + "$"))
                        .Select(n => n.Id)).SelectMany(m => m).Distinct().ToArray();
                    IRepository<TModuleFunction, Guid> moduleFunctionRepository = scopedProvider.GetService<IRepository<TModuleFunction, Guid>>();
                    functionIds = moduleFunctionRepository.Query(m => moduleIds.Contains(m.ModuleId)).Select(m => m.FunctionId).Distinct().ToArray();
                }
                if (functionIds.Length > 0)
                {
                    _cache.Set(key, functionIds);
                }
            }
            return functionIds;
        }
    }
}