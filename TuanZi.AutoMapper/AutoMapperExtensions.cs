using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TuanZi.Mapping;

namespace TuanZi.AutoMapper
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Map all types with the MapTo/MapFrom attributes to the type specified in MapTo/MapFrom.
        /// </summary>
        /// <param name="assembly">The assembly to search for types.</param>
        /// <param name="mapperConfiguration">The mapper configuration.</param>
        public static void MapTypes(this Assembly assembly, IMapperConfigurationExpression mapperConfiguration)
        {
            var assemblyTypes = assembly.GetTypes();
            var mappedTypes = 
                assemblyTypes.Select(t => new
                {
                    Type = t,
                    MapToAttributes = t.GetCustomAttributes(typeof(MapToAttribute), true).Cast<MapToAttribute>(),
                    MapFromAttributes = t.GetCustomAttributes(typeof(MapFromAttribute), true).Cast<MapFromAttribute>(),
                })
                .Where(t => t.MapToAttributes.Any() || t.MapFromAttributes.Any());
            
            foreach (var type in mappedTypes)
            {
                ProcessMapToAttributes(type.Type, type.MapToAttributes, mapperConfiguration, assemblyTypes);
                ProcessMapFromAttributes(type.Type, type.MapFromAttributes, mapperConfiguration, assemblyTypes);
            }
        }

        public static void MapTypes(this Type[] types, IMapperConfigurationExpression mapperConfiguration)
        {
            var mappedTypes =
                types.Select(t => new
                {
                    Type = t,
                    MapToAttributes = t.GetCustomAttributes(typeof(MapToAttribute), true).Cast<MapToAttribute>(),
                    MapFromAttributes = t.GetCustomAttributes(typeof(MapFromAttribute), true).Cast<MapFromAttribute>(),
                })
                .Where(t => t.MapToAttributes.Any() || t.MapFromAttributes.Any());

            foreach (var type in mappedTypes)
            {
                ProcessMapToAttributes(type.Type, type.MapToAttributes, mapperConfiguration, types);
                ProcessMapFromAttributes(type.Type, type.MapFromAttributes, mapperConfiguration, types);
            }
        }

        private static void ProcessMapFromAttributes(
            Type targetType, 
            IEnumerable<MapFromAttribute> mapFromAttributes, 
            IMapperConfigurationExpression mapperConfiguration, 
            Type[] types)
        {
            foreach (var mapFromAttribute in mapFromAttributes)
            {
                var sourceType = mapFromAttribute.SourceType;
                Mapptivate(mapFromAttribute, sourceType, targetType, mapperConfiguration, types, mapFromAttribute.ReverseMap);
            }
        }
        
        private static void ProcessMapToAttributes(
            Type sourceType, 
            IEnumerable<MapToAttribute> mapToAttributes, 
            IMapperConfigurationExpression mapperConfiguration, 
            Type[] types)
        {
            foreach (var mapToAttribute in mapToAttributes)
            {
                var targetType = mapToAttribute.TargetType;
                Mapptivate(mapToAttribute, sourceType, targetType, mapperConfiguration, types, mapToAttribute.ReverseMap);
            }
        }

        private static PropertyMapInfo[] GetMappedProperties(Type[] typesToSearch, Type sourceType, Type targetType)
        {
            var possibleSourceTypes = typesToSearch.Where(sourceType.IsAssignableFrom).ToArray();
            var sourceTypeMappings = GetSourceTypeMappings(possibleSourceTypes, targetType);
            var targetTypeMappings = GetTargetTypeMappings(typesToSearch, sourceType, targetType);

            return new[]
            {
                sourceTypeMappings,
                targetTypeMappings,
            }.SelectMany(p => p).ToArray();
        }

        private static PropertyMapInfo[] GetTargetTypeMappings(Type[] typesToSearch, Type sourceType, Type targetType)
        {
            return typesToSearch
                .Where(targetType.IsAssignableFrom)
                .SelectMany(t =>
                    t.GetProperties().Select(p =>
                    {
                        var mapToAndFromPropertyAttributes = p.GetCustomAttributes<MapToAndFromPropertyAttribute>();
                        var foo = new
                        {
                            Property = p,
                            MappingAttributes = new IEnumerable<MapPropertyAttribute>[]
                            {
                                p.GetCustomAttributes<IgnoreMapFromAttribute>()
                                    .Where(pt => pt.SourceType.IsAssignableFrom(sourceType)),
                                mapToAndFromPropertyAttributes
                                    .Where(pt => pt.SourceOrTargetType.IsAssignableFrom(sourceType)),
                                p.GetCustomAttributes<MapFromPropertyAttribute>()
                                    .Where(pt => pt.SourceType.IsAssignableFrom(sourceType))
                            }.SelectMany(m => m).ToArray(),
                            SourceType = t
                        };
                        return foo;
                    })
                )
                .SelectMany(p => p.MappingAttributes.SelectMany(a => a.GetPropertyMapInfo(p.Property, p.SourceType)))
                .ToArray();
        }

        internal static PropertyMapInfo[] GetSourceTypeMappings(Type[] possibleSourceTypes, Type targetType)
        {
            return possibleSourceTypes
                .SelectMany(t =>
                    t.GetProperties().Select(p => new 
                    {
                        Property = p,
                        MappingAttributes = new IEnumerable<MapPropertyAttribute>[]
                        {
                            p.GetCustomAttributes<MapToPropertyAttribute>()
                                .Where(pt => pt.TargetType.IsAssignableFrom(targetType)),
                            p.GetCustomAttributes<MapToAndFromPropertyAttribute>()
                                .Where(pt => pt.SourceOrTargetType.IsAssignableFrom(targetType)),
                            t.GetCustomAttributes<IgnoreMapToPropertiesAttribute>()
                                .Where(pt => pt.PropertyName == p.Name && pt.TargetType.IsAssignableFrom(targetType))
                        }.SelectMany(m => m)
                    })
                )
                .SelectMany(p => p.MappingAttributes.SelectMany(a => a.GetPropertyMapInfo(p.Property)))
                .ToArray();
        }
        
        /// <summary>
        /// All these map puns are giving me a headache.
        /// </summary>
        internal static void Mapptivate(MapAttribute mapToAttribute, Type sourceType, Type targetType, IMapperConfigurationExpression mapperConfiguration, Type[] types, bool reverseMap)
        {
            var mappedProperties = GetMappedProperties(types, sourceType, targetType);
            var mappingExpression = MapTypes(sourceType, targetType, mappedProperties, mapperConfiguration);
            
            //if a ConfigureMapping method is defined, call it
            var configureMappingGenericMethod = GetConfigureMappingGenericMethod(mapToAttribute, sourceType, targetType);
            configureMappingGenericMethod?.Invoke(mapToAttribute, new[] { mappingExpression });

            if (reverseMap)
            {
                Mapptivate(mapToAttribute, targetType, sourceType, mapperConfiguration, types, false);
            }
        }

        private static MethodInfo GetConfigureMappingGenericMethod(MapAttribute mapToAttribute, Type sourceType, Type targetType)
        {
            return mapToAttribute.GetType()
                .GetMethod("ConfigureMapping",
                           new[] { typeof(IMappingExpression<,>).MakeGenericType(sourceType, targetType) });
        }

        internal static object MapTypes(Type sourceType,
            Type targetType,
            PropertyMapInfo[] propertyMapInfos,
            IMapperConfigurationExpression mapperConfiguration)
        {
            var createMapMethodInfo = GenericCreateMap.MakeGenericMethod(sourceType, targetType);

            //actually create the mapping
            var mappingExpression = createMapMethodInfo.Invoke(mapperConfiguration, new object[] { });
            Debug.WriteLine($"Mapping created for source type {sourceType.Name} to target type {targetType.Name}");

            var mapObjectExpression = MapProperties(sourceType, targetType, propertyMapInfos, mappingExpression);
            Expression.Lambda(mapObjectExpression).Compile().DynamicInvoke();
            return mappingExpression;
        }

        internal static Expression MapProperties(Type sourceType, Type targetType, PropertyMapInfo[] propertyMapInfos, object mappingExpression)
        {
            Expression mapObjectExpression = Expression.Constant(mappingExpression);

            foreach (var propMapInfo in propertyMapInfos)
            {
                var sourceTypeParameter = Expression.Parameter(sourceType);
                var targetPropertyInfo = propMapInfo.TargetPropertyInfo;
                
                var memberConfigType = 
                    propMapInfo.UseSourceMember 
                        ? typeof(ISourceMemberConfigurationExpression)
                        : typeof(IMemberConfigurationExpression<,,>).MakeGenericType(sourceType, targetType, typeof(object));
                var memberConfigTypeParameter = Expression.Parameter(memberConfigType);
                
                var forMemberMethodName =
                    propMapInfo.UseSourceMember
                        ? nameof(IMappingExpression<object, object>.ForSourceMember)
                        : nameof(IMappingExpression<object, object>.ForMember);

                var memberOptions =
                    propMapInfo.IgnoreMapping
                        ? GetIgnoreCall(memberConfigTypeParameter)
                        : GetMapFromCall(memberConfigTypeParameter, targetPropertyInfo, propMapInfo, sourceTypeParameter);

                var lambdaExpression = Expression.Lambda(memberOptions, memberConfigTypeParameter);
                var forMemberMethodExpression =
                    Expression.Call(
                        mapObjectExpression,
                        forMemberMethodName,
                        Type.EmptyTypes,
                        Expression.Constant(targetPropertyInfo?.Name ?? propMapInfo.SourcePropertyInfos.Single().Name),
                        lambdaExpression);

                mapObjectExpression = forMemberMethodExpression;
            }
            return mapObjectExpression;
        }

        private static MethodCallExpression GetIgnoreCall(ParameterExpression memberConfigTypeParameter)
        {
            return Expression.Call(memberConfigTypeParameter, nameof(IMemberConfigurationExpression.Ignore),
                Type.EmptyTypes);
        }

        private static MethodCallExpression GetMapFromCall(ParameterExpression memberConfigTypeParameter, PropertyInfo targetPropertyInfo, PropertyMapInfo propMapInfo, ParameterExpression sourceTypeParameter)
        {
            var sourcePropertyInfos = propMapInfo.SourcePropertyInfos;
            var targetPropertyName = targetPropertyInfo.Name;

            Debug.WriteLine(
                $"-- Mapping property {string.Join(".", sourcePropertyInfos.Select(s => s.Name))} to target type {targetPropertyName}");

            var finalSourcePropertyType = sourcePropertyInfos.Last().PropertyType;
            var propertyExpression =
                sourcePropertyInfos.Aggregate<PropertyInfo, Expression>(null, (current, prop) => current == null
                    ? Expression.Property(sourceTypeParameter, prop)
                    : Expression.Property(current, prop));

            return Expression.Call(memberConfigTypeParameter, nameof(IMemberConfigurationExpression.MapFrom),
                new Type[] {finalSourcePropertyType},
                Expression.Lambda(
                    propertyExpression,
                    sourceTypeParameter
                ));
        }

        static AutoMapperExtensions()
        {
            GenericCreateMap =
                typeof(IProfileExpression).GetMethods()
                    .Single(m => m.Name == nameof(IMapperConfigurationExpression.CreateMap) &&
                                 m.IsGenericMethodDefinition &&
                                 m.GetGenericArguments().Length == 2 &&
                                 !m.GetParameters().Any());
        }

        private static MethodInfo GenericCreateMap { get; }
    }
}