using System;


namespace TuanZi.Mapping
{
    public class MapToAttribute : Attribute
    {
        public MapToAttribute(params Type[] targetTypes)
        {
            Check.NotNull(targetTypes, nameof(targetTypes));
            TargetTypes = targetTypes;
        }

        public Type[] TargetTypes { get; }
    }
}