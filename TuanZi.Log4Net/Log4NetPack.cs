using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using TuanZi.Core.Packs;


namespace TuanZi.Log4Net
{
    public class Log4NetPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Core;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider, Log4NetLoggerProvider>();
            return services;
        }
    }
}
