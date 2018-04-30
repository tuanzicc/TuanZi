using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TuanZi.Collections;
using TuanZi.Core.Modules;
using TuanZi.Core.Options;


namespace TuanZi.Core.Builders
{
    public class TuanBuilder : ITuanBuilder
    {
        public TuanBuilder()
        {
            Modules = new List<Type>();
            ExcludedModules = new List<Type>();
        }

        public IEnumerable<Type> Modules { get; private set; }

        public IEnumerable<Type> ExcludedModules { get; private set; }

        public Action<TuanOptions> OptionsAction { get; private set; }

        public ITuanBuilder AddModule<TModule>() where TModule : TuanModule
        {
            List<Type> list = Modules.ToList();
            list.AddIfNotExist(typeof(TModule));
            Modules = list;
            return this;
        }

        public ITuanBuilder ExcludeModule<TModule>() where TModule : TuanModule
        {
            List<Type> list = ExcludedModules.ToList();
            list.AddIfNotExist(typeof(TModule));
            ExcludedModules = list;
            return this;
        }

        public ITuanBuilder AddOptions(Action<TuanOptions> optionsAction)
        {
            Check.NotNull(optionsAction, nameof(optionsAction));
            OptionsAction = optionsAction;
            return this;
        }
    }
}
