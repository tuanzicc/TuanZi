﻿using System;
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
        private readonly IDistributedCache _cache;

        public OnlineUserCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public virtual OnlineUser GetOrRefresh(string userName)
        {
            string key = $"Identity_OnlineUser_{userName}";

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
            return _cache.Get<OnlineUser>(key,
                () =>
                {
                    return ServiceLocator.Instance.ExcuteScopedWork<OnlineUser>(provider =>
                    {
                        UserManager<TUser> userManager = provider.GetService<UserManager<TUser>>();
                        TUser user = userManager.FindByNameAsync(userName).Result;
                        if (user == null)
                        {
                            return null;
                        }
                        IList<string> roles = userManager.GetRolesAsync(user).Result;

                        RoleManager<TRole> roleManager = provider.GetService<RoleManager<TRole>>();
                        bool isAdmin = roleManager.Roles.Any(m => roles.Contains(m.Name) && m.IsAdmin);

                        return GetOnlineUser(user, roles.ToArray(), isAdmin);
                    });
                },
                options);
        }

        public async Task<OnlineUser> GetOrRefreshAsync(string userName)
        {
            string key = $"Identity_OnlineUser_{userName}";

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
            return await _cache.GetAsync<OnlineUser>(key,
                () =>
                {
                    return ServiceLocator.Instance.ExcuteScopedWorkAsync<OnlineUser>(async provider =>
                    {
                        UserManager<TUser> userManager = provider.GetService<UserManager<TUser>>();
                        TUser user = await userManager.FindByNameAsync(userName);
                        if (user == null)
                        {
                            return null;
                        }
                        IList<string> roles = await userManager.GetRolesAsync(user);

                        RoleManager<TRole> roleManager = provider.GetService<RoleManager<TRole>>();
                        bool isAdmin = roleManager.Roles.Any(m => roles.Contains(m.Name) && m.IsAdmin);

                        return GetOnlineUser(user, roles.ToArray(), isAdmin);
                    });
                },
                options);
        }

        public void Remove(params string[] userNames)
        {
            foreach (string userName in userNames)
            {
                string key = $"Identity_OnlineUser_{userName}";
                _cache.Remove(key);
            }
        }

        private static OnlineUser GetOnlineUser(TUser user, string[] roles, bool isAdmin)
        {
            return new OnlineUser()
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                NickName = user.NickName,
                Email = user.Email,
                HeadImg = user.HeadImg,
                IsAdmin = isAdmin,
                Roles = roles
            };
        }
    }
}