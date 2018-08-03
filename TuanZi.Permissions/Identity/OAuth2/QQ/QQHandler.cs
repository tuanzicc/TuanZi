using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;


namespace Tuan.Identity.OAuth2.QQ
{
    internal class QQHandler : OAuthHandler<QQOptions>
    {
        public QQHandler(IOptionsMonitor<QQOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            string openIdEndpoint = QueryHelpers.AddQueryString(Options.OpenIdEndpoint, "access_token", tokens.AccessToken);
            HttpResponseMessage openIdResponse = await Backchannel.GetAsync(openIdEndpoint, Context.RequestAborted);
            if (!openIdResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"未能检索QQ Connect的OpenId(返回状态码:{openIdResponse.StatusCode})，请检查access_token是正确。");
            }
            JObject json = JObject.Parse(企鹅的返回不拘一格传入这里统一转换为JSON(await openIdResponse.Content.ReadAsStringAsync()));
            string openId = GetOpenId(json);

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {  "openid", openId},
                {  "access_token", tokens.AccessToken },
                {  "oauth_consumer_key", Options.ClientId }
            };
            string userInformationEndpoint = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, parameters);
            HttpResponseMessage userInformationResponse = await Backchannel.GetAsync(userInformationEndpoint, Context.RequestAborted);
            if (!userInformationResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"未能检索QQ Connect的用户信息(返回状态码:{userInformationResponse.StatusCode})，请检查参数是否正确。");
            }

            JObject payload = JObject.Parse(await userInformationResponse.Content.ReadAsStringAsync());
            payload.Add("openid", openId);
            OAuthCreatingTicketContext context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload);
            context.RunClaimActions();
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {  "grant_type", "authorization_code" },
                {  "client_id", Options.ClientId },
                {  "client_secret", Options.ClientSecret },
                {  "code", code},
                {  "redirect_uri", redirectUri}
            };

            string endpoint = QueryHelpers.AddQueryString(Options.TokenEndpoint, parameters);

            HttpResponseMessage response = await Backchannel.GetAsync(endpoint, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                return OAuthTokenResponse.Failed(new Exception("获取QQ Connect Access Token出错。"));
            }
            JObject payload = JObject.Parse(企鹅的返回不拘一格传入这里统一转换为JSON(await response.Content.ReadAsStringAsync()));
            return OAuthTokenResponse.Success(payload);
        }

        private static string 企鹅的返回不拘一格传入这里统一转换为JSON(string text)
        {
            Regex openIdRegex = new Regex("callback\\((?<json>[ -~]+)\\);");
            if (openIdRegex.IsMatch(text))
            {
                return openIdRegex.Match(text).Groups["json"].Value;
            }

            Regex tokenRegex = new Regex("^access_token=.{1,}&expires_in=.{1,}&refresh_token=.{1,}");
            if (tokenRegex.IsMatch(text))
            {
                return "{\"" + text.Replace("=", "\":\"").Replace("&", "\",\"") + "\"}";
            }
            return "{}";
        }

        private static string GetOpenId(JObject json)
        {
            return json.Value<string>("openid");
        }
    }
}
