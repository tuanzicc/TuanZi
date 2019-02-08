using System;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TuanZi.Core.Packs;
using TuanZi.Data;
using TuanZi.Exceptions;
using TuanZi.Extensions;


namespace TuanZi.Redis
{
    public abstract class RedisPackCore : TuanPack
    {
        private bool _enabled = false;

        public override PackLevel Level => PackLevel.Framework;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            IConfiguration configuration = services.GetConfiguration();
            string config = configuration["Tuan:Redis:Configuration"];
            if (config.IsNullOrEmpty())
            {
                throw new TuanException("The configuration of the Redis node in the configuration file cannot be empty.");
            }
            string name = configuration["Tuan:Redis:InstanceName"].CastTo("RedisName");
            _enabled = configuration["Tuan:Redis:Enabled"].CastTo(false);
            if (_enabled)
            {
                services.RemoveAll(typeof(IDistributedCache));
                services.AddDistributedRedisCache(opts =>
                {
                    opts.Configuration = config;
                    opts.InstanceName = name;
                });
            }

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            IsEnabled = _enabled;
        }
    }
}