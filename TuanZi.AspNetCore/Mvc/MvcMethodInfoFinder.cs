using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TuanZi.Collections;
using TuanZi.Reflection;

namespace TuanZi.AspNetCore.Mvc
{
    public class MvcMethodInfoFinder : IMethodInfoFinder
    {
        public MethodInfo[] Find(Type type, Func<MethodInfo, bool> predicate)
        {
            return FindAll(type).Where(predicate).ToArray();
        }

        public MethodInfo[] FindAll(Type type)
        {
            List<Type> types = new List<Type>();
            while (IsController(type))
            {
                types.AddIfNotExist(type);
                type = type?.BaseType;
                if (type?.Name == "Controller" || type?.Name == "ControllerBase")
                {
                    break;
                }
            }

            return types.SelectMany(m => m.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)).ToArray();
        }

        private static bool IsController(Type type)
        {
            return type != null && type.IsClass && type.IsPublic && !type.ContainsGenericParameters
                && !type.IsDefined(typeof(NonControllerAttribute)) && (type.Name.EndsWith("Controller") || type.Name.EndsWith("ControllerBase")
                    || type.IsDefined(typeof(ControllerAttribute)));
        }
    }
}
