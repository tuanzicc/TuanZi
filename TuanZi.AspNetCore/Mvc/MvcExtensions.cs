using System;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;


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
    }
}