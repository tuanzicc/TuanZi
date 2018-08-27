using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuanZi.Core.Builders;
using TuanZi.Data;
using TuanZi.Reflection;


namespace TuanZi.Core.Packs
{
    public class TuanPackManager : ITuanPackManager
    {
        private readonly ITuanBuilder _builder;
        private readonly List<TuanPack> _sourcePacks;
        private readonly TuanPackTypeFinder _typeFinder;

        public TuanPackManager()
        {
            _builder = Singleton<ITuanBuilder>.Instance;
            IAllAssemblyFinder allAssemblyFinder = Singleton<IAllAssemblyFinder>.Instance;
            _typeFinder = new TuanPackTypeFinder(allAssemblyFinder);
            _sourcePacks = new List<TuanPack>();
            LoadedPacks = new List<TuanPack>();
        }

        public IEnumerable<TuanPack> SourcePacks => _sourcePacks;

        public IEnumerable<TuanPack> LoadedPacks { get; private set; }

        public virtual IServiceCollection LoadPacks(IServiceCollection services)
        {
            Type[] packTypes = _typeFinder.FindAll();
            _sourcePacks.Clear();
            _sourcePacks.AddRange(packTypes.Select(m => (TuanPack)Activator.CreateInstance(m)));
            List<TuanPack> packs;
            if (_builder.Packs.Any())
            {
                packs = _sourcePacks.Where(m => m.Level == PackLevel.Core)
                    .Union(_sourcePacks.Where(m => _builder.Packs.Contains(m.GetType()))).Distinct().ToList();
                IEnumerable<Type> dependModuleTypes = packs.SelectMany(m => m.GetDependModuleTypes());
                packs = packs.Union(_sourcePacks.Where(m => dependModuleTypes.Contains(m.GetType()))).Distinct().ToList();
            }
            else
            {
                packs = _sourcePacks.ToList();
                packs.RemoveAll(m => _builder.ExcludedPacks.Contains(m.GetType()));
            }
            packs = packs.OrderBy(m => m.Level).ThenBy(m => m.Order).ToList();
            LoadedPacks = packs;

            foreach (TuanPack pack in LoadedPacks)
            {
                services = pack.AddServices(services);
            }

            return services;
        }

        public virtual void UsePack(IServiceProvider provider)
        {
            ILogger logger = provider.GetLogger<TuanPackManager>();
            logger.LogInformation("Tuan framework initialization begins");
            DateTime dtStart = DateTime.Now;

            foreach (TuanPack pack in LoadedPacks)
            {
                pack.UsePack(provider);
                logger.LogInformation($"Pack {pack.GetType()} Loaded");
            }

            TimeSpan ts = DateTime.Now.Subtract(dtStart);
            logger.LogInformation($"Tuan framework is initialized and takes time:{ts:g}");
        }
    }


}