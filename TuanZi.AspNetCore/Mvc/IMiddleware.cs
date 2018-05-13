using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TuanZi.AspNetCore.Mvc
{
    public interface IMiddleware
    {
        Task Invoke(HttpContext context);
    }
}
