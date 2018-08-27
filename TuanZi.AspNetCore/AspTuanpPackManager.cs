using System;
using System.ComponentModel;
using System.Diagnostics;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.Core.Builders;
using TuanZi.Core.Packs;
using TuanZi.Exceptions;
using TuanZi.Reflection;


namespace TuanZi.AspNetCore
{
    public class AspTuanPackManager : TuanPackManager, IAspUsePack
    {
        

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void UsePack(IServiceProvider provider)
        {
            IHostingEnvironment environment = provider.GetService<IHostingEnvironment>();
            if (environment != null)
            {
                throw new TuanException("Currently in the AspNetCore environment, use UsePack (IApplicationBuilder) for initialization");
            }

            base.UsePack(provider);
        }

        public void UsePack(IApplicationBuilder app)
        {
            ILogger logger = app.ApplicationServices.GetLogger<AspTuanPackManager>();
            logger.LogInformation("Tuan framework initialization begins");
            DateTime dtStart = DateTime.Now;

            foreach (TuanPack pack in LoadedPacks)
            {
                pack.UsePack(app.ApplicationServices);

                if (pack is AspTuanPack aspPack)
                {
                    aspPack.UsePack(app);
                }
            }

            TimeSpan ts = DateTime.Now.Subtract(dtStart);
            logger.LogInformation($"Tuan framework is initialized and takes time:{ts:g}");
        }
    }
}