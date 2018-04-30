using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Builders;
using TuanZi.Reflection;


namespace TuanZi.Core.Modules
{
    public class TuanModuleManager
    {
        private readonly ITuanBuilder _builder;
        private readonly TuanModuleTypeFinder _typeFinder;
        private readonly List<TuanModule> _sourceModules;

        public TuanModuleManager(ITuanBuilder builder, IAllAssemblyFinder allAssemblyFinder)
        {
            _builder = builder;
            _typeFinder = new TuanModuleTypeFinder(allAssemblyFinder);
            _sourceModules = new List<TuanModule>();
            LoadedModules = new List<TuanModule>();
        }

        public IEnumerable<TuanModule> SourceModules
        {
            get { return _sourceModules; }
        }

        public IEnumerable<TuanModule> LoadedModules { get; private set; }

        public IServiceCollection LoadModules(IServiceCollection services)
        {
            Type[] moduleTypes = _typeFinder.FindAll();
            _sourceModules.Clear();
            _sourceModules.AddRange(moduleTypes.Select(m => (TuanModule)Activator.CreateInstance(m)));
            List<TuanModule> modules;
            if (_builder.Modules.Any())
            {
                modules = _sourceModules.Where(m => m.Level == ModuleLevel.Core)
                    .Union(_sourceModules.Where(m => _builder.Modules.Contains(m.GetType()))).Distinct().ToList();
                IEnumerable<Type> dependModuleTypes = modules.SelectMany(m => m.GetDependModuleTypes());
                modules = modules.Union(_sourceModules.Where(m => dependModuleTypes.Contains(m.GetType()))).Distinct().ToList();
            }
            else
            {
                modules = _sourceModules.ToList();
                modules.RemoveAll(m => _builder.ExcludedModules.Contains(m.GetType()));
            }
            modules = modules.OrderBy(m => m.Level).ThenBy(m => m.Order).ToList();
            LoadedModules = modules;

            foreach (TuanModule module in LoadedModules)
            {
                services = module.AddServices(services);
            }

            return services;
        }

        public void UseModules(IServiceProvider provider)
        {
            foreach (TuanModule module in LoadedModules)
            {
                module.UseModule(provider);
            }
        }
    }
}