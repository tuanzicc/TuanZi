using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

using TuanZi.Core.Functions;
using TuanZi.Entity;
using TuanZi.Secutiry.Claims;


namespace TuanZi.Secutiry
{
  
    public abstract class FunctionAuthorizationBase<TFunction> : IFunctionAuthorization
        where TFunction : class, IFunction, IEntity<Guid>
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
            if(!(principal.Identity is ClaimsIdentity identity))
            {
                return new AuthorizationResult(AuthorizationStatus.Error, "The current user ID IIdentity is not in the correct format. Only the Identity of the ClaimsIdentity type is supported.");
            }
            if (!(function is TFunction func))
            {
                return new AuthorizationResult(AuthorizationStatus.Error, $"The type of function to detect is '{function.GetType()}', not a required '{typeof(TFunction)}' type");
            }
            string[] userRoleNames = identity.GetRoles().ToArray();
            if (userRoleNames.Contains(SuperRoleName))
            {
                return AuthorizationResult.OK;
            }

            string[] functionRoleNames = FunctionAuthCache.GetFunctionRoles(func.Id);
            if (userRoleNames.Intersect(functionRoleNames).Any())
            {
                return AuthorizationResult.OK;
            }
            Guid[] functionIds = FunctionAuthCache.GetUserFunctions(identity.GetUserName());
            if (functionIds.Contains(func.Id))
            {
                return AuthorizationResult.OK;
            }
            return new AuthorizationResult(AuthorizationStatus.Forbidden);
        }
    }
}