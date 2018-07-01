using System;
using System.Collections.Generic;

using TuanZi.Core.Packs;
using TuanZi.Core.Options;


namespace TuanZi.Core.Builders
{
    public interface ITuanBuilder
    {
        IEnumerable<Type> Packs { get; }

        IEnumerable<Type> ExcludedPacks { get; }

        Action<TuanOptions> OptionsAction { get; }

        ITuanBuilder AddPack<TPack>() where TPack : TuanPack;

        ITuanBuilder ExcludePack<TPack>() where TPack : TuanPack;

        ITuanBuilder AddOptions(Action<TuanOptions>optionsAction);
    }
}