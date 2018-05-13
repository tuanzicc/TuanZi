using System.Threading.Tasks;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using TuanZi.AspNetCore.Http;
using TuanZi.AspNetCore.UI;
using TuanZi.Exceptions;


namespace TuanZi.AspNetCore.Mvc
{
    public class JsonExceptionHandlerMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;

        public JsonExceptionHandlerMiddleware(RequestDelegate next)
        {
            Check.NotNull(next, nameof(next));

            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Features[typeof(IExceptionHandlerFeature)] is IExceptionHandlerFeature feature
                && (context.Request.IsAjaxRequest() || context.Request.IsJsonContextType()))
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "applicaton/json";
                return context.Response.WriteAsync(new AjaxResult(feature.Error.FormatMessage(), AjaxResultType.Error).ToJsonString());
            }

            return _next(context);
        }
    }
}