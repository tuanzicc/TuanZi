using System.Linq;
using System.Security.Claims;

using Hangfire.Dashboard;

using Microsoft.AspNetCore.Http;

using TuanZi.Secutiry.Claims;


namespace TuanZi.Hangfire
{
    public class RoleDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleDashboardAuthorizationFilter(string[] roles)
        {
            _roles = roles;
        }

        public bool Authorize(DashboardContext context)
        {
            HttpContext httpContext = context.GetHttpContext();
            ClaimsPrincipal principal = httpContext.User;
            ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return false;
            }

            string[] roles = identity.GetRoles();
            return _roles.Intersect(roles).Any();
        }
    }
}