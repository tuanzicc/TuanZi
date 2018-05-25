using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TuanZi.AspNetCore.Http;
using TuanZi.AspNetCore.UI;

namespace TuanZi.AspNetCore.Mvc
{
    public class NodeExceptionHandlerMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<NodeExceptionHandlerMiddleware> _logger;

        public NodeExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            Check.NotNull(next, nameof(next));

            _next = next;
            _logger = loggerFactory.CreateLogger<NodeExceptionHandlerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex, ex.Message);
                if (context.Request.IsAjaxRequest() || context.Request.IsJsonContextType())
                {
                    context.Response.StatusCode = 500;
                    context.Response.Clear();
                    context.Response.ContentType = "applicaton/json";
                    await context.Response.WriteAsync(new AjaxResult(ex.Message, AjaxResultType.Error).ToJsonString());
                    return;
                }
                throw;
            }
        }
    }
}
