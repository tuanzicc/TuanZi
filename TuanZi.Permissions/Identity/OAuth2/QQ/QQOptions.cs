using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;


namespace Tuan.Identity.OAuth2.QQ
{
    public class QQOptions : OAuthOptions
    {
        public QQOptions()
        {
            CallbackPath = new PathString("/signin-qq");
            AuthorizationEndpoint = QQDefaults.AuthorizationEndpoint;
            TokenEndpoint = QQDefaults.TokenEndpoint;
            UserInformationEndpoint = QQDefaults.UserInformationEndpoint;
            OpenIdEndpoint = QQDefaults.OpenIdEndpoint;

            Scope.Add("get_user_info"); 

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
            ClaimActions.MapJsonKey("urn:qq:figure", "figureurl_qq_1");
        }

        public string OpenIdEndpoint { get; }

        public string AppId
        {
            get { return ClientId; }
            set { ClientId = value; }
        }

        public string AppKey
        {
            get { return ClientSecret; }
            set { ClientSecret = value; }
        }
    }
}