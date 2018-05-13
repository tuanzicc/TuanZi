using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using TuanZi.Core.Functions;
using TuanZi.Exceptions;

namespace TuanZi.AspNetCore.Mvc
{
    public static class MvcExtensions
    {
        public static bool IsController(this Type type)
        {
            return IsController(type.GetTypeInfo());
        }

        public static bool IsController(this TypeInfo typeInfo)
        {
            return typeInfo.IsClass && !typeInfo.IsAbstract && (typeInfo.IsPublic && !typeInfo.ContainsGenericParameters)
                && (!typeInfo.IsDefined(typeof(NonControllerAttribute)) && (typeInfo.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                    || typeInfo.IsDefined(typeof(ControllerAttribute))));
        }

        public static string GetAreaName(this ActionContext context)
        {
            string area = null;
            if (context.RouteData.Values.TryGetValue("area", out object value))
            {
                area = (string)value;
                if (area.IsNullOrWhiteSpace())
                {
                    area = null;
                }
            }
            return area;
        }

        public static string GetControllerName(this ActionContext context)
        {
            return context.RouteData.Values["controller"].ToString();
        }

        public static string GetActionName(this ActionContext context)
        {
            return context.RouteData.Values["action"].ToString();
        }

        public static IFunction GetExecuteFunction(this ActionContext context)
        {
            const string key = TuanConstants.CurrentMvcFunctionKey;
            IDictionary<object, object> items = context.HttpContext.Items;
            if (items.ContainsKey(key))
            {
                return items[key] as IFunction;
            }
            string area = context.GetAreaName();
            string controller = context.GetControllerName();
            string action = context.GetActionName();
            IMvcFunctionHandler functionHandler = ServiceLocator.Instance.GetService<IMvcFunctionHandler>();
            if (functionHandler == null)
            {
                throw new TuanException("IMvcFunctionHandler fails to parse when getting the function being executed");
            }
            IFunction function = functionHandler.GetFunction(area, controller, action);
            if (function != null)
            {
                items.Add(key, function);
            }
            return function;
        }

        public static IFunction GetExecuteFunction(this ControllerBase controller)
        {
            return controller.ControllerContext.GetExecuteFunction();
        }
    }
}