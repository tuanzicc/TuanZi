using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;

using TuanZi.Core.Functions;
using TuanZi.Core.Modules;
using TuanZi.Exceptions;
using TuanZi.Reflection;


namespace TuanZi.AspNetCore.Mvc
{
    public class MvcModuleInfoPicker : ModuleInfoPickerBase<Function>
    {
        public MvcModuleInfoPicker(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        protected override ModuleInfo[] GetModules(Type type, string[] existPaths)
        {
            ModuleInfoAttribute infoAttr = type.GetAttribute<ModuleInfoAttribute>();
            ModuleInfo info = new ModuleInfo()
            {
                Name = infoAttr.Name ?? GetName(type),
                Code = infoAttr.Code ?? type.Name.Replace("Controller", ""),
                Order = infoAttr.Order,
                Position = GetPosition(type, infoAttr.Position),
                PositionName = infoAttr.PositionName
            };
            List<ModuleInfo> infos = new List<ModuleInfo>() { info };
            if (infoAttr.Position != null)
            {
                info = new ModuleInfo()
                {
                    Name = infoAttr.PositionName ?? infoAttr.Position,
                    Code = infoAttr.Position,
                    Position = GetPosition(type, null)
                };
                if (!existPaths.Contains($"{info.Position}.{info.Code}"))
                {
                    infos.Insert(0, info);
                }
            }
            string area = type.GetAttribute<AreaAttribute>(true)?.RouteValue ?? "Site";
            string name = area;
            info = new ModuleInfo()
            {
                Name = name ?? area,
                Code = area,
                Position = "Root",
                PositionName = area
            };
            if (!existPaths.Contains($"{info.Position}.{info.Code}"))
            {
                infos.Insert(0, info);
            }

            return infos.ToArray();
        }

        protected override ModuleInfo GetModule(MethodInfo method, ModuleInfo typeInfo, int index)
        {
            ModuleInfoAttribute infoAttr = method.GetAttribute<ModuleInfoAttribute>();
            ModuleInfo info = new ModuleInfo()
            {
                Name = infoAttr.Name ?? method.GetDescription() ?? method.Name,
                Code = infoAttr.Code ?? method.Name,
                Order = infoAttr.Order > 0 ? infoAttr.Order : index + 1,
            };
            string controller = method.DeclaringType?.Name.Replace("Controller", "");
            info.Position = $"{typeInfo.Position}.{controller}";
            string area = method.DeclaringType.GetAttribute<AreaAttribute>(true)?.RouteValue;
            List<IFunction> dependOnFunctions = new List<IFunction>()
            {
                FunctionHandler.GetFunction(area, controller, method.Name)
            };
            DependOnFunctionAttribute[] dependOnAttrs = method.GetAttributes<DependOnFunctionAttribute>();
            foreach (DependOnFunctionAttribute dependOnAttr in dependOnAttrs)
            {
                string darea = dependOnAttr.Area == null ? area : dependOnAttr.Area == string.Empty ? null : dependOnAttr.Area;
                string dcontroller = dependOnAttr.Controller ?? controller;
                IFunction function = FunctionHandler.GetFunction(darea, dcontroller, dependOnAttr.Action);
                if (function == null)
                {
                    throw new TuanException($"The dependency function '{darea}/{dcontroller}/{dependOnAttr.Action}' of the function '{area}/{controller}/{method.Name}' could not be found");
                }
                dependOnFunctions.Add(function);
            }
            info.DependOnFunctions = dependOnFunctions.ToArray();

            return info;
        }

        private static string GetName(Type type)
        {
            string name = type.GetDescription();
            if (name == null)
            {
                return type.Name.Replace("Controller", "");
            }
            if (name.Contains("-"))
            {
                name = name.Split('-').Last();
            }
            return name;
        }

        private static string GetPosition(Type type, string attrPosition)
        {
            string area = type.GetAttribute<AreaAttribute>(true)?.RouteValue;
            if (area == null)
            {
                return attrPosition == null
                    ? "Root.Site"
                    : $"Root.Site.{attrPosition}";
            }
            return attrPosition == null
                ? $"Root.{area}"
                : $"Root.{area}.{attrPosition}";
        }
    }
}