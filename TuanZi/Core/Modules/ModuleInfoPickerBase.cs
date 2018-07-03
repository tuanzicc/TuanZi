﻿using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.Logging;

using TuanZi.Core.Functions;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.Reflection;


namespace TuanZi.Core.Modules
{
    public abstract class ModuleInfoPickerBase<TFunction> : IModuleInfoPicker
        where TFunction : class, IEntity<Guid>, IFunction, new()
    {
        protected ModuleInfoPickerBase()
        {
            ServiceLocator locator = ServiceLocator.Instance;
            Logger = locator.GetService<ILoggerFactory>().CreateLogger(GetType());
            FunctionHandler = locator.GetService<IFunctionHandler>();
        }

        protected ILogger Logger { get; }

        protected IFunctionHandler FunctionHandler { get; }

        public ModuleInfo[] Pickup()
        {
            Check.NotNull(FunctionHandler, nameof(FunctionHandler));
            Type[] moduleTypes = FunctionHandler.FunctionTypeFinder.Find(type => type.HasAttribute<ModuleInfoAttribute>());
            ModuleInfo[] modules = GetModules(moduleTypes);
            return modules;
        }

        protected virtual ModuleInfo[] GetModules(Type[] moduleTypes)
        {
            List<ModuleInfo> infos = new List<ModuleInfo>();
            foreach (Type moduleType in moduleTypes)
            {
                ModuleInfo typeInfo = GetModule(moduleType);
                infos.Add(typeInfo);
                MethodInfo[] methods = FunctionHandler.MethodInfoFinder.Find(moduleType, type => type.HasAttribute<ModuleInfoAttribute>());
                for (int index = 0; index < methods.Length; index++)
                {
                    ModuleInfo methodInfo = GetModule(methods[index], typeInfo, index);
                    infos.Add(methodInfo);
                }
            }
            return infos.ToArray();
        }

        protected abstract ModuleInfo GetModule(Type type);

        protected abstract ModuleInfo GetModule(MethodInfo method, ModuleInfo typeInfo, int index);
    }
}