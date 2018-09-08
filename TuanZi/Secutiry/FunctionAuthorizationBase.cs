using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

using TuanZi.Core.Functions;
using TuanZi.Data;
using TuanZi.Entity;
using TuanZi.Secutiry.Claims;


namespace TuanZi.Secutiry
{
    public abstract class FunctionAuthorizationBase : IFunctionAuthorization
    {
        protected FunctionAuthorizationBase(IFunctionAuthCache functionAuthCache)
        {
            FunctionAuthCache = functionAuthCache;
            SuperRoleName = "System administrator";
        }

        protected IFunctionAuthCache FunctionAuthCache { get; }

        protected virtual string SuperRoleName { get; }

        public AuthorizationResult Authorize(IFunction function, IPrincipal principal)
        {
            return AuthorizeCore(function, principal);
        }

        public virtual string[] GetOkRoles(IFunction function, IPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated)
            {
                return new string[0];
            }

            string[] userRoles = principal.Identity.GetRoles();
            if (function.AccessType != FunctionAccessType.RoleLimit)
            {
                return userRoles;
            }
            string[] functionRoles = FunctionAuthCache.GetFunctionRoles(function.Id);

            return userRoles.Intersect(functionRoles).ToArray();
        }

        protected virtual AuthorizationResult AuthorizeCore(IFunction function, IPrincipal principal)
        {
            if (function == null)
            {
                return new AuthorizationResult(AuthorizationStatus.NoFound);
            }
            if (function.IsLocked)
            {
                return new AuthorizationResult(AuthorizationStatus.Locked, $"Function '{function.Name}' has been locked");
            }
            if (function.AccessType == FunctionAccessType.Anonymouse)
            {
                return AuthorizationResult.OK;
            }
            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                return new AuthorizationResult(AuthorizationStatus.Unauthorized);
            }
            if (function.AccessType == FunctionAccessType.Logined)
            {
                return AuthorizationResult.OK;
            }
            return AuthorizeRoleLimit(function, principal);
        }

        protected virtual AuthorizationResult AuthorizeRoleLimit(IFunction function, IPrincipal principal)
        {
            if (!(principal.Identity is ClaimsIdentity identity))
            {
                return new AuthorizationResult(AuthorizationStatus.Error, "The current user ID IIdentity is not in the correct format. Only the Identity of the ClaimsIdentity type is supported.");
            }
            string[] userRoleNames = identity.GetRoles().ToArray();
            AuthorizationResult result = AuthorizeRoleNames(function, userRoleNames);
            if (result.IsOk)
            {
                return result;
            }
            result = AuthorizeUserName(function, principal.Identity.GetUserName());
            return result;
        }

        protected virtual AuthorizationResult AuthorizeRoleNames(IFunction function, params string[] roleNames)
        {
            Check.NotNull(roleNames, nameof(roleNames));

            if (roleNames.Length == 0)
            {
                return new AuthorizationResult(AuthorizationStatus.Forbidden);
            }
            if (function.AccessType != FunctionAccessType.RoleLimit || roleNames.Contains(SuperRoleName))
            {
                return AuthorizationResult.OK;
            }
            string[] functionRoleNames = FunctionAuthCache.GetFunctionRoles(function.Id);
            if (roleNames.Intersect(functionRoleNames).Any())
            {
                return AuthorizationResult.OK;
            }
            return new AuthorizationResult(AuthorizationStatus.Forbidden);
        }

        protected virtual AuthorizationResult AuthorizeUserName(IFunction function, string userName)
        {
            if (function.AccessType != FunctionAccessType.RoleLimit)
            {
                return AuthorizationResult.OK;
            }

            Guid[] functionIds = FunctionAuthCache.GetUserFunctions(userName);
            if (functionIds.Contains(function.Id))
            {
                return AuthorizationResult.OK;
            }
            return new AuthorizationResult(AuthorizationStatus.Forbidden);
        }
    }
}