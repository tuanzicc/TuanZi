using System;


namespace TuanZi.Core.Packs
{
    public class DependsOnPacksAttribute : Attribute
    {
        public DependsOnPacksAttribute(params Type[] dependedModuleTypes)
        {
            DependedModuleTypes = dependedModuleTypes;
        }

        public Type[] DependedModuleTypes { get; }
    }
}