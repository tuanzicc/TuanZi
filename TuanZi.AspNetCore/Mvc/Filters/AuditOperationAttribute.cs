using System;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Audits;
using TuanZi.Core.Functions;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.Exceptions;
using TuanZi.Secutiry.Claims;


namespace TuanZi.AspNetCore.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuditOperationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IServiceProvider provider = context.HttpContext.RequestServices;
            IFunction function = context.GetExecuteFunction();
            if (function == null)
            {
                return;
            }
            ScopedDictionary dict = provider.GetService<ScopedDictionary>();
            dict.Function = function;
            IFunctionAuthorization functionAuthorization = provider.GetService<IFunctionAuthorization>();
            string[] roleName = functionAuthorization.GetOkRoles(function, context.HttpContext.User);
            dict.DataAuthValidRoleNames = roleName;

            if (!function.AuditOperationEnabled)
            {
                return;
            }
            AuditOperationEntry operation = new AuditOperationEntry
            {
                FunctionName = function.Name,
                Ip = context.HttpContext.GetClientIp(),
                UserAgent = context.HttpContext.Request.Headers["User-Agent"].FirstOrDefault(),
                CreatedTime = DateTime.Now
            };
            if (context.HttpContext.User.Identity.IsAuthenticated && context.HttpContext.User.Identity is ClaimsIdentity identity)
            {
                operation.UserId = identity.GetUserId();
                operation.UserName = identity.GetUserName();
                operation.NickName = identity.GetNickName();
            }

            dict.AuditOperation = operation;
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            IServiceProvider provider = context.HttpContext.RequestServices;
            ScopedDictionary dict = provider.GetService<ScopedDictionary>();
            if (dict.AuditOperation?.FunctionName == null)
            {
                return;
            }
            dict.AuditOperation.EndedTime = DateTime.Now;
            IUnitOfWork unitOfWork = provider.GetService<IUnitOfWork>();
            unitOfWork?.Rollback();

            IAuditStore store = provider.GetService<IAuditStore>();
            store?.Save(dict.AuditOperation);
            unitOfWork?.Commit();
        }

    }
}

