using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TuanZi.AspNetCore.Mvc.Filters;
using TuanZi.Core;
using TuanZi.Core.Functions;
using TuanZi.Exceptions;
using TuanZi.Reflection;


namespace TuanZi.AspNetCore.Mvc
{
    public class MvcFunctionHandler : FunctionHandlerBase<Function, MvcFunctionHandler>, IMvcFunctionHandler
    {
        public MvcFunctionHandler(IServiceProvider applicationServiceProvider)
            : base(applicationServiceProvider)
        {
            FunctionTypeFinder = new MvcControllerTypeFinder(AllAssemblyFinder);
            MethodInfoFinder = new PublicInstanceMethodInfoFinder();
        }

        public override IFunctionTypeFinder FunctionTypeFinder { get; }

        public override IMethodInfoFinder MethodInfoFinder { get; }

        protected override Function GetFunction(Type controllerType)
        {
            if (!controllerType.IsController())
            {
                throw new TuanException($"Type '{controllerType.FullName}' is not an MVC controller type");
            }
            FunctionAccessType accessType = controllerType.HasAttribute<LoginedAttribute>() || controllerType.HasAttribute<AuthorizeAttribute>()
               ? FunctionAccessType.Logined
               : controllerType.HasAttribute<RoleLimitAttribute>()
                   ? FunctionAccessType.RoleLimit
                   : FunctionAccessType.Anonymouse;
            Function function = new Function()
            {
                Name = controllerType.GetDescription(),
                Area = GetArea(controllerType),
                Controller = controllerType.Name.Replace("Controller", string.Empty),
                IsController = true,
                AccessType = accessType
            };
            return function;
        }

        protected override Function GetFunction(Function typeFunction, MethodInfo method)
        {
            FunctionAccessType accessType = method.HasAttribute<LoginedAttribute>() || method.HasAttribute<AuthorizeAttribute>()
                ? FunctionAccessType.Logined
                : method.HasAttribute<AllowAnonymousAttribute>()
                    ? FunctionAccessType.Anonymouse
                    : method.HasAttribute<RoleLimitAttribute>()
                        ? FunctionAccessType.RoleLimit
                        : typeFunction.AccessType;
            Function function = new Function()
            {
                Name = $"{typeFunction.Name}-{method.GetDescription()}",
                Area = typeFunction.Area,
                Controller = typeFunction.Controller,
                Action = method.Name,
                AccessType = accessType,
                IsController = false,
                IsAjax = method.HasAttribute<AjaxOnlyAttribute>()
            };
            return function;
        }

        protected override string GetArea(Type type)
        {
            AreaAttribute attribute = type.GetAttribute<AreaAttribute>(true);
            return attribute?.RouteValue;
        }

        protected override bool IsIgnoreMethod(Function action, MethodInfo method, IEnumerable<Function> functions)
        {
            bool flag = base.IsIgnoreMethod(action, method, functions);
            return flag && method.HasAttribute<HttpPostAttribute>() || method.HasAttribute<NonActionAttribute>();
        }
    }
}