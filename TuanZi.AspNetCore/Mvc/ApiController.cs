using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.AspNetCore.Mvc.Filters;

namespace TuanZi.AspNetCore.Mvc
{
    [AuditOperation]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class ApiController : Controller
    {

        protected ILogger Logger => HttpContext.RequestServices.GetLogger(GetType());
    }
}
