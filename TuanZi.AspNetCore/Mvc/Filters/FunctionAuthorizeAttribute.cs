using System;
using System.Linq;
using System.Net;
using System.Security.Principal;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

using TuanZi.AspNetCore.Http;
using TuanZi.AspNetCore.UI;
using TuanZi.Core.Functions;
using TuanZi.Secutiry;

using AuthorizationResult = TuanZi.Secutiry.AuthorizationResult;


namespace TuanZi.AspNetCore.Mvc.Filters
{
    public class FunctionAuthorizeAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Check.NotNull(context, nameof(context));
            IFunction function = context.GetExecuteFunction();
            AuthorizationResult result = AuthorizeCore(context, function);
            if (!result.IsOk)
            {
                HandleUnauthorizedRequest(context, result);
            }
        }

        protected virtual AuthorizationResult AuthorizeCore(AuthorizationFilterContext context, IFunction function)
        {
            IPrincipal user = context.HttpContext.User;
            IServiceProvider provider = context.HttpContext.RequestServices;
            IFunctionAuthorization authorization = provider.GetService<IFunctionAuthorization>();
            AuthorizationResult result = authorization.Authorize(function, user);
            return result;
        }

        protected virtual void HandleUnauthorizedRequest(AuthorizationFilterContext context, AuthorizationResult result)
        {
            bool isJsRequest = context.HttpContext.Request.IsAjaxRequest() || context.HttpContext.Request.IsJsonContextType();

            AuthorizationStatus status = result.ResultType;
            switch (status)
            {
                case AuthorizationStatus.Unauthorized:
                    context.Result = isJsRequest
                        ? (IActionResult)new JsonResult(new AjaxResult(result.Message, AjaxResultType.UnAuth))
                        : new UnauthorizedResult();
                    break;
                case AuthorizationStatus.Forbidden:
                    context.Result = isJsRequest
                        ? (IActionResult)new JsonResult(new AjaxResult(result.Message, AjaxResultType.Forbidden))
                        : new StatusCodeResult(403);
                    break;
                case AuthorizationStatus.NoFound:
                    context.Result = isJsRequest
                        ? (IActionResult)new JsonResult(new AjaxResult(result.Message, AjaxResultType.NoFound))
                        : new StatusCodeResult(404);
                    break;
                case AuthorizationStatus.Locked:
                    context.Result = isJsRequest
                        ? (IActionResult)new JsonResult(new AjaxResult(result.Message, AjaxResultType.Locked))
                        : new StatusCodeResult(423);
                    break;
                case AuthorizationStatus.Error:
                    context.Result = isJsRequest
                        ? (IActionResult)new JsonResult(new AjaxResult(result.Message, AjaxResultType.Error))
                        : new StatusCodeResult(500);
                    break;
            }
            if (isJsRequest)
            {
                context.HttpContext.Response.StatusCode = 200;
            }
        }

    }
}
