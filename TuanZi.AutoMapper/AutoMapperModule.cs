using System;
using System.Linq;

using AutoMapper;
using AutoMapper.Configuration;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Modules;
using TuanZi.Mapping;

using IMapper = TuanZi.Mapping.IMapper;


namespace TuanZi.AutoMapper
{
    public class AutoMapperModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Framework;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<MapperConfigurationExpression>(new MapperConfigurationExpression());

            services.AddSingleton<IMapFromAttributeTypeFinder, MapFromAttributeTypeFinder>();
            services.AddSingleton<IMapToAttributeTypeFinder, MapToAttributeTypeFinder>();
            services.AddSingleton<IMapTuple, MapAttributeProfile>();
            services.AddSingleton<IMapper, AutoMapperMapper>();

            return services;
        }

        public override void UseModule(IServiceProvider provider)
        {
            MapperConfigurationExpression cfg = provider.GetService<MapperConfigurationExpression>() ?? new MapperConfigurationExpression();

            IMapTuple[] tuples = provider.GetServices<IMapTuple>().ToArray();
            foreach (IMapTuple mapTuple in tuples)
            {
                mapTuple.CreateMap();
                cfg.AddProfile(mapTuple as Profile);
            }

           // var profiles = provider.GetServices<IMapProfileTypeFinder>();

      


            Mapper.Initialize(cfg);

            IMapper mapper = provider.GetService<IMapper>();
            MapperExtensions.SetMapper(mapper);

            IsEnabled = true;
        }
    }
}