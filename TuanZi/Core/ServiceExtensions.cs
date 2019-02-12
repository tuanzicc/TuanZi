using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TuanZi.Core.Builders;
using TuanZi.Core.Packs;
using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.Reflection;
using TuanZi.Data;
using TuanZi.Entity;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTuan<TTuanPackManager>(this IServiceCollection services, Action<ITuanBuilder> builderAction = null)
            where TTuanPackManager : ITuanPackManager, new()
        {
            Check.NotNull(services, nameof(services));

            services.TryAddSingleton<IAllAssemblyFinder>(new AppDomainAllAssemblyFinder());

            ITuanBuilder builder = services.GetSingletonInstanceOrNull<ITuanBuilder>() ?? new TuanBuilder();
            builderAction?.Invoke(builder);
            services.TryAddSingleton<ITuanBuilder>(builder);

            TTuanPackManager manager = new TTuanPackManager();
            services.AddSingleton<ITuanPackManager>(manager);
            manager.LoadPacks(services);
            return services;
        }

        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            return services.GetSingletonInstance<IConfiguration>();
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

        public static IDbContext GetDbContext<TEntity, TKey>(this IServiceProvider provider) where TEntity : IEntity<TKey>
        {
            IUnitOfWorkManager unitOfWorkManager = provider.GetService<IUnitOfWorkManager>();
            return unitOfWorkManager.GetDbContext<TEntity, TKey>();
        }

        public static HttpContext GetHttpContext(this IServiceProvider provider)
        {
            return provider.GetService<HttpContext>();
        }

        public static IServiceProvider UseTuan(this IServiceProvider provider)
        {
            ITuanPackManager packManager = provider.GetService<ITuanPackManager>();
            packManager.UsePack(provider);
            return provider;
        }


    }
}