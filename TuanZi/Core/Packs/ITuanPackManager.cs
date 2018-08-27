using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;


namespace TuanZi.Core.Packs
{
    public interface ITuanPackManager
    {
        IEnumerable<TuanPack> SourcePacks { get; }

        IEnumerable<TuanPack> LoadedPacks { get; }

        IServiceCollection LoadPacks(IServiceCollection services);

        void UsePack(IServiceProvider provider);
    }
}