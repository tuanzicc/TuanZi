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
        /// <summary>
        /// Creates a <see cref="T:System.Security.Claims.ClaimsIdentity" /> from a <see cref="T:System.IdentityModel.Tokens.Jwt.JwtSecurityToken" />.
        /// </summary>
        /// <param name="jwtToken">The <see cref="T:System.IdentityModel.Tokens.Jwt.JwtSecurityToken" /> to use as a <see cref="T:System.Security.Claims.Claim" /> source.</param>
        /// <param name="issuer">The value to set <see cref="P:System.Security.Claims.Claim.Issuer" /></param>
        /// <param name="validationParameters"> Contains parameters for validating the token.</param>
        /// <returns>A <see cref="T:System.Security.Claims.ClaimsIdentity" /> containing the <see cref="P:System.IdentityModel.Tokens.Jwt.JwtSecurityToken.Claims" />.</returns>
        protected override ClaimsIdentity CreateClaimsIdentity(JwtSecurityToken jwtToken,
            string issuer,
            TokenValidationParameters validationParameters)
        {
            ClaimsIdentity identity = base.CreateClaimsIdentity(jwtToken, issuer, validationParameters);

            if (identity.IsAuthenticated)
            {
                IOnlineUserCache onlineUserCache = ServiceLocator.Instance.GetService<IOnlineUserCache>();
                OnlineUser user = onlineUserCache.GetOrRefresh(identity.Name);
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

            return identity;
        }
    }
}