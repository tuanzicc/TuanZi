using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace TuanZi.Identity
{
    public class OnlineUserProvider<TUser, TUserKey, TRole, TRoleKey> : IOnlineUserProvider
        where TUser : UserBase<TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TRole : RoleBase<TRoleKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        public virtual async Task<OnlineUser> Create(IServiceProvider provider, string userName)
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
            return new OnlineUser()
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                NickName = user.NickName,
                Email = user.Email,
                HeadImg = user.HeadImg,
                IsAdmin = isAdmin,
                Roles = roles.ToArray()
            };
        }
    }
}