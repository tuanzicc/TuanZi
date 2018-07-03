using System;
using System.Linq;

using AutoMapper;
using AutoMapper.Configuration;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;
using TuanZi.Mapping;

using IMapper = TuanZi.Mapping.IMapper;


namespace TuanZi.AutoMapper
{
    public class AutoMapperPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Framework;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton(new MapperConfigurationExpression());
            services.AddSingleton<IMapFromAttributeTypeFinder, MapFromAttributeTypeFinder>();
            services.AddSingleton<IMapToAttributeTypeFinder, MapToAttributeTypeFinder>();
            services.AddSingleton<IMapper, AutoMapperMapper>();

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            MapperConfigurationExpression cfg = provider.GetService<MapperConfigurationExpression>() ?? new MapperConfigurationExpression();
            

            var mapFromAttributeTypeFinder = provider.GetService<IMapFromAttributeTypeFinder>();
            mapFromAttributeTypeFinder.FindAll().MapTypes(cfg);

            var mapToAttributeTypeFinder = provider.GetService<IMapToAttributeTypeFinder>();
            mapToAttributeTypeFinder.FindAll().MapTypes(cfg);

            Mapper.Initialize(cfg);

            IMapper mapper = provider.GetService<IMapper>();
            MapperExtensions.SetMapper(mapper);

            IsEnabled = true;
        }
    }
}