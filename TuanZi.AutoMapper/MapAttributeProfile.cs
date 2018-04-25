using System;
using System.Collections.Generic;

using AutoMapper;

using TuanZi.Collections;
using TuanZi.Mapping;
using TuanZi.Reflection;


namespace TuanZi.AutoMapper
{
    public class MapAttributeProfile : Profile, IMapTuple
    {
        private readonly IMapFromAttributeTypeFinder _mapFromAttributeTypeFinder;
        private readonly IMapToAttributeTypeFinder _mapToAttributeTypeFinder;

        public MapAttributeProfile(IMapFromAttributeTypeFinder mapFromAttributeTypeFinder, IMapToAttributeTypeFinder mapToAttributeTypeFinder)
        {
            _mapFromAttributeTypeFinder = mapFromAttributeTypeFinder;
            _mapToAttributeTypeFinder = mapToAttributeTypeFinder;
        }

        public void CreateMap()
        {
            List<(Type Source, Type Target)> tuples = new List<(Type Source, Type Target)>();

            Type[] types = _mapFromAttributeTypeFinder.FindAll(true);
            foreach (Type targetType in types)
            {
                MapFromAttribute attribute = targetType.GetAttribute<MapFromAttribute>();
                foreach (Type sourceType in attribute.SourceTypes)
                {
                    var tuple = ValueTuple.Create(sourceType, targetType);
                    tuples.AddIfNotExist(tuple);
                }
            }

            types = _mapToAttributeTypeFinder.FindAll(true);
            foreach (Type sourceType in types)
            {
                MapToAttribute attribute = sourceType.GetAttribute<MapToAttribute>();
                foreach (Type targetType in attribute.TargetTypes)
                {
                    var tuple = ValueTuple.Create(sourceType, targetType);
                    tuples.AddIfNotExist(tuple);
                }
            }

            foreach ((Type Source, Type Target) tuple in tuples)
            {
                CreateMap(tuple.Source, tuple.Target);
            }
        }
    }
}