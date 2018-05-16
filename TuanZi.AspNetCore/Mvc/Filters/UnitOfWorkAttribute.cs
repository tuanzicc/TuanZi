using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using TuanZi.AspNetCore.UI;
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
            if (context.Result is JsonResult result)
            {
                if (result.Value is AjaxResult ajax && ajax.Error())
                {
                    return;
                }
            }
            _unitOfWork?.Commit();
        }
    }
}