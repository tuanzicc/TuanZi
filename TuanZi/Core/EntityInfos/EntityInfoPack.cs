using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;


namespace TuanZi.Core.EntityInfos
{
    public class EntityInfoPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override void UsePack(IServiceProvider provider)
        {
            IEntityInfoHandler handler = provider.GetService<IEntityInfoHandler>();
            handler.Initialize();
            IsEnabled = true;
        }
    }
}