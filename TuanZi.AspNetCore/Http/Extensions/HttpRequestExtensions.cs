using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;


namespace TuanZi.AspNetCore.Http
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            Check.NotNull(request, nameof(request));
            if (request.Headers != null)
            {
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }
            return false;
        }

        public static string Params(this HttpRequest request, string key)
        {
            if (request.Query.ContainsKey(key))
            {
                return request.Query[key];
            }
            if (request.HasFormContentType)
            {
                return request.Form[key];
            }
            return null;
        }
    }
}