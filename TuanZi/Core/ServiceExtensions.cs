using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TuanZi.Core.Builders;
using TuanZi.Core.Packs;
using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.Reflection;
using TuanZi.Data;
using Microsoft.Extensions.Logging;
using TuanZi.Entity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTuan<TTuanPackManager>(this IServiceCollection services, Action<ITuanBuilder> builderAction = null)
            where TTuanPackManager : ITuanPackManager, new()
        {
            Check.NotNull(services, nameof(services));

            if (Singleton<IAllAssemblyFinder>.Instance == null)
            {
                Singleton<IAllAssemblyFinder>.Instance = new AppDomainAllAssemblyFinder();
            }

            ITuanBuilder builder = new TuanBuilder();
            if (builderAction != null)
            {
                builderAction(builder);
            }
            Singleton<ITuanBuilder>.Instance = builder;
            TTuanPackManager manager = new TTuanPackManager();
            services.AddSingleton<ITuanPackManager>(manager);
            manager.LoadPacks(services);
            return services;
        }

        public static TuanOptions GetTuanOptions(this IServiceProvider provider)
        {
            return provider.GetService<IOptions<TuanOptions>>()?.Value;
        }

        public static ILogger<T> GetLogger<T>(this IServiceProvider provider)
        {
            ILoggerFactory factory = provider.GetService<ILoggerFactory>();
            return factory.CreateLogger<T>();
        }

        public static ILogger GetLogger(this IServiceProvider provider, Type type)
        {
            ILoggerFactory factory = provider.GetService<ILoggerFactory>();
            return factory.CreateLogger(type);
        }

        public static ILogger GetLogger(this IServiceProvider provider, string name)
        {
            ILoggerFactory factory = provider.GetService<ILoggerFactory>();
            return factory.CreateLogger(name);
        }

        public static IUnitOfWork GetUnitOfWork<TEntity, TKey>(this IServiceProvider provider) where TEntity : IEntity<TKey>
        {
            IUnitOfWorkManager unitOfWorkManager = provider.GetService<IUnitOfWorkManager>();
            return unitOfWorkManager.GetUnitOfWork<TEntity, TKey>();
        }

        public static IServiceProvider UseTuan(this IServiceProvider provider)
        {
            ITuanPackManager packManager = provider.GetService<ITuanPackManager>();
            packManager.UsePack(provider);
            return provider;
        }
    }
}