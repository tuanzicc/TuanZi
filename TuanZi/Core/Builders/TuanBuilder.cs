using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TuanZi.Collections;
using TuanZi.Core.Packs;
using TuanZi.Core.Options;
using TuanZi.Data;

namespace TuanZi.Core.Builders
{
    public class TuanBuilder : ITuanBuilder
    {
        public TuanBuilder()
        {
            Packs = new List<Type>();
            ExcludedPacks = new List<Type>();
        }

        public IEnumerable<Type> Packs { get; private set; }

        public IEnumerable<Type> ExcludedPacks { get; private set; }

        public Action<TuanOptions> OptionsAction { get; private set; }

        public ITuanBuilder AddPack<TPack>() where TPack : TuanPack
        {
            List<Type> list = Packs.ToList();
            list.AddIfNotExist(typeof(TPack));
            Packs = list;
            return this;
        }

        public ITuanBuilder ExcludePack<TPack>() where TPack : TuanPack
        {
            List<Type> list = ExcludedPacks.ToList();
            list.AddIfNotExist(typeof(TPack));
            ExcludedPacks = list;
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
