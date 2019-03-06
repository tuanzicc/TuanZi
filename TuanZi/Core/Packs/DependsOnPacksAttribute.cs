using System;


namespace TuanZi.Core.Packs
{
    public class DependsOnPacksAttribute : Attribute
    {
        public DependsOnPacksAttribute(params Type[] dependedPackTypes)
        {
            DependedPackTypes = dependedPackTypes;
        }

        public Type[] DependedPackTypes { get; }
    }
}