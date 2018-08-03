using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Functions;
using TuanZi.Exceptions;
using TuanZi.Secutiry;
using TuanZi.Extensions;
using TuanZi.Dependency;

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
            return typeInfo.IsClass && !typeInfo.IsAbstract && typeInfo.IsPublic && !typeInfo.ContainsGenericParameters
                && !typeInfo.IsDefined(typeof(NonControllerAttribute)) && (typeInfo.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                    || typeInfo.IsDefined(typeof(ControllerAttribute)));
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
            IServiceProvider provider = context.HttpContext.RequestServices;
            ScopedDictionary dict = provider.GetService<ScopedDictionary>();
            if (dict.Function != null)
            {
                return dict.Function;
            }
            string area = context.GetAreaName();
            string controller = context.GetControllerName();
            string action = context.GetActionName();
            IFunctionHandler functionHandler = provider.GetService<IFunctionHandler>();
            if (functionHandler == null)
            {
                throw new TuanException("IFunctionHandler fails to parse when getting the function being executed");
            }
            IFunction function = functionHandler.GetFunction(area, controller, action);
            if (function != null)
            {
                dict.Function = function;
            }
            return function;
        }

        public static IFunction GetExecuteFunction(this ControllerBase controller)
        {
            return controller.ControllerContext.GetExecuteFunction();
        }

        public static IFunction GetFunction(this ControllerBase controller, string url)
        {
            url = url.StartsWith("https://") || url.StartsWith("http://")
                ? new Uri(url).AbsolutePath : !url.StartsWith("/") ? $"/{url}" : url;
            IServiceProvider services = controller.HttpContext.RequestServices;
            IHttpContextFactory factory = services.GetService<IHttpContextFactory>();
            HttpContext httpContext = factory.Create(controller.HttpContext.Features);
            httpContext.Request.Path = url;
            httpContext.Request.Method = "POST";
            RouteContext routeContext = new RouteContext(httpContext);
            IRouteCollection router = controller.RouteData.Routers.OfType<IRouteCollection>().FirstOrDefault();
            if (router == null)
            {
                return null;
            }
            router.RouteAsync(routeContext).Wait();
            if (routeContext.Handler == null)
            {
                return null;
            }
            RouteValueDictionary dict = routeContext.RouteData.Values;
            string areaName = dict.GetOrDefault("area")?.ToString();
            string controllerName = dict.GetOrDefault("controller")?.ToString();
            string actionName = dict.GetOrDefault("action")?.ToString();
            IFunctionHandler handler = services.GetService<IFunctionHandler>();
            return handler?.GetFunction(areaName, controllerName, actionName);
        }

        public static bool CheckFunctionAuth(this Controller controller, string url)
        {
            IFunction function = controller.GetFunction(url);
            if (function == null)
            {
                return false;
            }
            IFunctionAuthorization authorization = controller.HttpContext.RequestServices.GetService<IFunctionAuthorization>();
            return authorization.Authorize(function, controller.User).IsOk;
        }

        public static bool CheckFunctionAuth(this Controller controller, string actionName, string controllerName, string areaName = null)
        {
            IServiceProvider services = controller.HttpContext.RequestServices;
            IFunctionHandler functionHandler = services.GetService<IFunctionHandler>();
            IFunction function = functionHandler?.GetFunction(areaName, controllerName, actionName);
            if (function == null)
            {
                return false;
            }
            IFunctionAuthorization authorization = services.GetService<IFunctionAuthorization>();
            return authorization.Authorize(function, controller.User).IsOk;
        }
    }
}