using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuanZi.Core.Builders;
using TuanZi.Dependency;


namespace TuanZi.Core.Packs
{
    public class TuanPackManager : ITuanPackManager
    {
        private readonly List<TuanPack> _sourcePacks;

        public TuanPackManager()
        {
            _sourcePacks = new List<TuanPack>();
            LoadedPacks = new List<TuanPack>();
        }

        public IEnumerable<TuanPack> SourcePacks => _sourcePacks;

        public IEnumerable<TuanPack> LoadedPacks { get; private set; }

        public virtual IServiceCollection LoadPacks(IServiceCollection services)
        {
            ITuanPackTypeFinder packTypeFinder =
                services.GetOrAddTypeFinder<ITuanPackTypeFinder>(assemblyFinder => new TuanPackTypeFinder(assemblyFinder));
            Type[] packTypes = packTypeFinder.FindAll();
            _sourcePacks.Clear();
            _sourcePacks.AddRange(packTypes.Select(m => (TuanPack)Activator.CreateInstance(m)));

            ITuanBuilder builder = services.GetSingletonInstance<ITuanBuilder>();
            List<TuanPack> packs;
            if (builder.Packs.Any())
            {
                packs = _sourcePacks.Where(m => m.Level == PackLevel.Core)
                    .Union(_sourcePacks.Where(m => builder.Packs.Contains(m.GetType()))).Distinct().ToList();
                IEnumerable<Type> dependModuleTypes = packs.SelectMany(m => m.GetDependModuleTypes());
                packs = packs.Union(_sourcePacks.Where(m => dependModuleTypes.Contains(m.GetType()))).Distinct().ToList();
            }
            else
            {
                packs = _sourcePacks.ToList();
                packs.RemoveAll(m => builder.ExcludedPacks.Contains(m.GetType()));
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