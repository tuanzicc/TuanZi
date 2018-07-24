using System;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core;
using TuanZi.Core.Packs;


namespace TuanZi.Identity
{
    public abstract class IdentityPackBase<TUserStore, TRoleStore, TUser, TRole, TUserKey, TRoleKey> : TuanPack
        where TUserStore : class, IUserStore<TUser>
        where TRoleStore : class, IRoleStore<TRole>
        where TUser : UserBase<TUserKey>
        where TRole : RoleBase<TRoleKey>
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserStore<TUser>, TUserStore>();
            services.AddScoped<IRoleStore<TRole>, TRoleStore>();

            services.AddTransient<IPrincipal>(provider =>
            {
                IHttpContextAccessor accessor = provider.GetService<IHttpContextAccessor>();
                return accessor?.HttpContext.User;
            });

            services.AddSingleton<IOnlineUserCache, OnlineUserCache<TUser, TUserKey, TRole, TRoleKey>>();

            Action<IdentityOptions> identityOptionsAction = IdentityOptionsAction();
            IdentityBuilder builder = services.AddIdentity<TUser, TRole>(identityOptionsAction);
            OnIdentityBuild(builder);

            Action<CookieAuthenticationOptions> cookieOptionsAction = CookieOptionsAction();
            if (cookieOptionsAction != null)
            {
                services.ConfigureApplicationCookie(cookieOptionsAction);
            }

            return services;
        }

        protected virtual Action<IdentityOptions> IdentityOptionsAction()
        {
            return null;
        }

        protected virtual Action<CookieAuthenticationOptions> CookieOptionsAction()
        {
            return null;
        }

        protected virtual IdentityBuilder OnIdentityBuild(IdentityBuilder builder)
        {
            return builder;
        }
    }
}