using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.AspNetCore.UI;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;


namespace TuanZi.AspNetCore.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    [Dependency(ServiceLifetime.Scoped, AddSelf = true)]
    public class UnitOfWorkAttribute : ActionFilterAttribute
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkAttribute(IServiceProvider serviceProvider)
        {
            _unitOfWorkManager = serviceProvider.GetService<IUnitOfWorkManager>();
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            ScopedDictionary dict = context.HttpContext.RequestServices.GetService<ScopedDictionary>();
            AjaxResultType type = AjaxResultType.Success;
            string message = null;
            if (context.Result is JsonResult result1)
            {
                if (result1.Value is AjaxResult ajax)
                {
                    type = ajax.Type;
                    message = ajax.Content;
                    if (ajax.Successed())
                    {
                        _unitOfWorkManager?.Commit();
                    }
                }

            }
            else if (context.Result is ObjectResult result2)
            {
                if (result2.Value is AjaxResult ajax)
                {
                    type = ajax.Type;
                    message = ajax.Content;
                    if (ajax.Successed())
                    {
                        _unitOfWorkManager?.Commit();
                    }
                }
                _unitOfWorkManager?.Commit();
            }
            else if (context.HttpContext.Response.StatusCode >= 400)
            {
                switch (context.HttpContext.Response.StatusCode)
                {
                    case 401:
                        type = AjaxResultType.UnAuth;
                        break;
                    case 403:
                        type = AjaxResultType.UnAuth;
                        break;
                    case 404:
                        type = AjaxResultType.UnAuth;
                        break;
                    case 423:
                        type = AjaxResultType.UnAuth;
                        break;
                    default:
                        type = AjaxResultType.Error;
                        break;
                }
            }
            else
            {
                type = AjaxResultType.Success;
                _unitOfWorkManager?.Commit();
            }
            if (dict.AuditOperation != null)
            {
                dict.AuditOperation.ResultType = type;
                dict.AuditOperation.Message = message;
            }
        }
    }
}