using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using TuanZi.Data;

namespace TuanZi.Secutiry.Claims
{
    public static class ClaimsIdentityExtensions
    {
        public static string GetClaimValueFirstOrDefault(this IIdentity identity, string type)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return null;
            }
            return claimsIdentity.FindFirst(type)?.Value;
        }

        public static string[] GetClaimValues(this IIdentity identity, string type)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return null;
            }
            return claimsIdentity.Claims.Where(m => m.Type == type).Select(m => m.Value).ToArray();
        }

        public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return default(T);
            }
            string value = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (value == null)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static string GetUserId(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return null;
            }
            return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetUserName(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return null;
            }
            return claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string GetEmail(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return null;
            }
            return claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string GetNickName(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return null;
            }
            return claimsIdentity.FindFirst(ClaimTypes.GivenName)?.Value;
        }

        public static void RemoveClaim(this IIdentity identity, string claimType)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return;
            }
            Claim claim = claimsIdentity.FindFirst(claimType);
            if (claim == null)
            {
                return;
            }
            claimsIdentity.RemoveClaim(claim);
        }

        public static string[] GetRoles(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return new string[0];
            }
            return claimsIdentity.FindAll(ClaimTypes.Role).SelectMany(m =>
            {
                string[] roles = m.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return roles;
            }).ToArray();
        }
    }
}