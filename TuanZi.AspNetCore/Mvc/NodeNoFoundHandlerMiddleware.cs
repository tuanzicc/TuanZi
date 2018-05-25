using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TuanZi.AspNetCore.Mvc
{
    public class NodeNoFoundHandlerMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;

        public NodeNoFoundHandlerMiddleware(RequestDelegate next)
        {
            Check.NotNull(next, nameof(next));

            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);
            if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value)
                && !context.Request.Path.Value.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                context.Request.Path = "/index.html";
                await _next(context);
            }
        }
    }
}
