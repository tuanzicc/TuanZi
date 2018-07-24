using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Dependency;

namespace TuanZi.AspNetCore.Mvc
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class ApiController : Controller
    {
        protected ApiController()
        {
            Logger = ServiceLocator.Instance.GetService<ILoggerFactory>().CreateLogger(GetType());
        }

        protected ILogger Logger { get; }
    }
}
