using System;
using System.Collections.Generic;

using TuanZi.Core.Modules;
using TuanZi.Core.Options;


namespace TuanZi.Core.Builders
{
    public interface ITuanBuilder
    {
        IEnumerable<Type> AddModules { get; }

        IEnumerable<Type> ExceptModules { get; }

        Action<TuanOptions> OptionsAction { get; }

        ITuanBuilder AddModule<TModule>() where TModule : TuanModule;

        ITuanBuilder ExceptModule<TModule>() where TModule : TuanModule;

        ITuanBuilder AddOptions(Action<TuanOptions>optionsAction);
    }
}