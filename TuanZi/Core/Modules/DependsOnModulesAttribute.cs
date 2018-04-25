using System;


namespace TuanZi.Core.Modules
{
    public class DependsOnModulesAttribute : Attribute
    {
        public DependsOnModulesAttribute(params Type[] dependedModuleTypes)
        {
            DependedModuleTypes = dependedModuleTypes;
        }

        public Type[] DependedModuleTypes { get; }
    }
}