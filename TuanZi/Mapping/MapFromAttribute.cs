using System;


namespace TuanZi.Mapping
{
    public class MapFromAttribute : Attribute
    {
        public MapFromAttribute(params Type[] sourceTypes)
        {
            Check.NotNull(sourceTypes, nameof(sourceTypes));
            SourceTypes = sourceTypes;
        }

        public Type[] SourceTypes { get; }
    }
}