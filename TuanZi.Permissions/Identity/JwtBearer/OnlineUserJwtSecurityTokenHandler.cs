using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

using TuanZi.Collections;
using TuanZi.Dependency;
using TuanZi.Secutiry.Claims;


namespace TuanZi.Identity.JwtBearer
{
    public class OnlineUserJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        protected override ClaimsIdentity CreateClaimsIdentity(JwtSecurityToken jwtToken,
            string issuer,
            TokenValidationParameters validationParameters)
        {
            ClaimsIdentity identity = base.CreateClaimsIdentity(jwtToken, issuer, validationParameters);

            if (identity.IsAuthenticated)
            {
                IOnlineUserCache onlineUserCache = ServiceLocator.Instance.GetService<IOnlineUserCache>();
                OnlineUser user = onlineUserCache.GetOrRefresh(identity.Name);
                if (user == null)
                {
                    return null;
                }
                identity.AddClaims(new[]
                {
                    new Claim(ClaimTypes.GivenName, user.NickName),
                    new Claim(ClaimTypes.Email, user.Email)
                });
                if (user.Roles.Length > 0)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, user.Roles.ExpandAndToString()));
                }
            }

            ScopedDictionary dict = ServiceLocator.Instance.GetService<ScopedDictionary>();
            dict.Identity = identity;
            return identity;
        }
    }
}