using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.AspNetCore.Http.Extensions
{
    public static class ServiceLocatorExtensions
    {
        public static HttpContext HttpContext(this ServiceLocator locator)
        {
            IHttpContextAccessor accessor = locator.GetService<IHttpContextAccessor>();
            return accessor?.HttpContext;
        }
    }
}
