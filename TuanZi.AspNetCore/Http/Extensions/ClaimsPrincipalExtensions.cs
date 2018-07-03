using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using TuanZi.Extensions;

namespace TuanZi.AspNetCore.Http
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static TKey GetUserId<TKey>(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return (principal.FindFirst(ClaimTypes.NameIdentifier)?.Value).CastTo<TKey>();
        }

        public static TKey GetAppId<TKey>(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return (principal.FindFirst(ClaimTypes.PrimaryGroupSid)?.Value).CastTo<TKey>();
        }

        public static TKey GetClaimValue<TKey>(this ClaimsPrincipal principal, string claimKey)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return (principal.FindFirst(claimKey)?.Value).CastTo<TKey>();
        }
    }
}
