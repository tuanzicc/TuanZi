using System;
using System.Collections.Generic;

using TuanZi.Core.Modules;
using TuanZi.Core.Options;


namespace TuanZi.Core.Builders
{
    public interface ITuanBuilder
    {
        IEnumerable<Type> Modules { get; }

        IEnumerable<Type> ExcludedModules { get; }

        Action<TuanOptions> OptionsAction { get; }

        ITuanBuilder AddModule<TModule>() where TModule : TuanModule;

        ITuanBuilder ExcludeModule<TModule>() where TModule : TuanModule;

        ITuanBuilder AddOptions(Action<TuanOptions>optionsAction);
    }
}