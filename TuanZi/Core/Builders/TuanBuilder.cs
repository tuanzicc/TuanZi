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
            AddModules = new List<Type>();
            ExceptModules = new List<Type>();
        }

        public IEnumerable<Type> AddModules { get; private set; }

        public IEnumerable<Type> ExceptModules { get; private set; }

        public Action<TuanOptions> OptionsAction { get; private set; }

        public ITuanBuilder AddModule<TModule>() where TModule : TuanModule
        {
            List<Type> list = AddModules.ToList();
            list.AddIfNotExist(typeof(TModule));
            AddModules = list;
            return this;
        }

        public ITuanBuilder ExceptModule<TModule>() where TModule : TuanModule
        {
            List<Type> list = ExceptModules.ToList();
            list.AddIfNotExist(typeof(TModule));
            ExceptModules = list;
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
