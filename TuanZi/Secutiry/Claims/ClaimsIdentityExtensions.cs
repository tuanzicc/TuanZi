using System.Linq;
using System.Security.Claims;


namespace TuanZi.Secutiry.Claims
{
    public static class ClaimsIdentityExtensions
    {
        public static string GetClaimValueFirstOrDefault(this ClaimsIdentity identity, string type)
        {
            Claim claim = identity.Claims.FirstOrDefault(m => m.Type == type);
            return claim?.Value;
        }

        public static string[] GetClaimValues(this ClaimsIdentity identity, string type)
        {
            return identity.Claims.Where(m => m.Type == type).Select(m => m.Value).ToArray();
        }
    }
}