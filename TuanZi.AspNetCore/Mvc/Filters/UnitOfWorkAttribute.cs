using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using TuanZi.AspNetCore.UI;
using TuanZi.Dependency;
using TuanZi.Entity;


namespace TuanZi.AspNetCore.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UnitOfWorkAttribute : ActionFilterAttribute
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkAttribute()
        {
            _unitOfWork = ServiceLocator.Instance.GetService<IUnitOfWork>();
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.Result is JsonResult result1)
            {
                if (result1.Value is AjaxResult ajax && !ajax.Successed())
                {
                    return;
                }
                _unitOfWork?.Commit();
                return;
            }
            if (context.Result is ObjectResult result2)
            {
                if (result2.Value is AjaxResult ajax && !ajax.Successed())
                {
                    return;
                }
                _unitOfWork?.Commit();
                return;
            }
            if (context.HttpContext.Response.StatusCode >= 400)
            {
                return;
            }
            _unitOfWork?.Commit();
        }
    }
}