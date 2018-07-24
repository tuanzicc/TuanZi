using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.Exceptions;


namespace TuanZi.Identity.JwtBearer
{
    public class JwtHelper
    {
        public static string CreateToken(Claim[] claims)
        {
            TuanOptions options = ServiceLocator.Instance.GetService<IOptions<TuanOptions>>().Value;
            JwtOptions jwtOptions = options.Jwt;
            string secret = jwtOptions.Secret;
            if (secret == null)
            {
                throw new TuanException("Secret is empty when creating JwtToken");
            }
            SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            DateTime now = DateTime.Now;
            DateTime expires = now.AddDays(7);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = jwtOptions.Audience,
                Issuer = jwtOptions.Issuer,
                SigningCredentials = credentials,
                NotBefore = now,
                IssuedAt = now,
                Expires = expires
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}