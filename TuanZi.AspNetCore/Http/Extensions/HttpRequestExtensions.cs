using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using TuanZi.Data;

namespace TuanZi.AspNetCore
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            Check.NotNull(request, nameof(request));
            bool? flag = request.Headers?["X-Requested-With"].ToString()?.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
            return flag.HasValue && flag.Value;
        }

        public static bool IsJsonContextType(this HttpRequest request)
        {
            Check.NotNull(request, nameof(request));
            return request.Headers?["Content-Type"].ToString()?.IndexOf("application/json", StringComparison.OrdinalIgnoreCase) > -1;
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

        public static string GetClientIp(this HttpContext context)
        {
            string ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
    }
}