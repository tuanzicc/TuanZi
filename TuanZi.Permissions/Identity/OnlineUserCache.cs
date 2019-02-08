using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Caching;
using TuanZi.Dependency;


namespace TuanZi.Identity
{
    public class OnlineUserCache<TUser, TUserKey, TRole, TRoleKey> : IOnlineUserCache
        where TUser : UserBase<TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TRole : RoleBase<TRoleKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDistributedCache _cache;

        public OnlineUserCache(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cache = serviceProvider.GetService<IDistributedCache>();
        }

        public virtual OnlineUser GetOrRefresh(string userName)
        {
            string key = $"Identity_OnlineUser_{userName}";

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
            return _cache.Get<OnlineUser>(key,
                () =>
                {
                    return _serviceProvider.ExecuteScopedWork<OnlineUser>(provider =>
                    {
                        IOnlineUserProvider onlineUserProvider = provider.GetService<IOnlineUserProvider>();
                        return onlineUserProvider.Create(provider, userName).Result;
                    });
                },
                options);
        }

        public virtual async Task<OnlineUser> GetOrRefreshAsync(string userName)
        {
            string key = $"Identity_OnlineUser_{userName}";

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
            return await _cache.GetAsync<OnlineUser>(key,
                () =>
                {
                    return _serviceProvider.ExecuteScopedWorkAsync<OnlineUser>(async provider =>
                    {
                        IOnlineUserProvider onlineUserProvider = provider.GetService<IOnlineUserProvider>();
                        return await onlineUserProvider.Create(provider, userName);
                    });
                },
                options);
        }

        public virtual void Remove(params string[] userNames)
        {
            foreach (string userName in userNames)
            {
                string key = $"Identity_OnlineUser_{userName}";
                _cache.Remove(key);
            }
        }
    }

}