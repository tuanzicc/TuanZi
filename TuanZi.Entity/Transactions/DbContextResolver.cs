using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Entity.Transactions;
using TuanZi.Exceptions;


namespace TuanZi.Entity
{
    public class DbContextResolver : IDbContextResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDbContext Resolve(DbContextResolveOptions resolveOptions)
        {
            Type dbContextType = resolveOptions.DbContextType;
            IDbContextOptionsBuilderCreator builderCreator = _serviceProvider.GetServices<IDbContextOptionsBuilderCreator>()
                .FirstOrDefault(m => m.Type == resolveOptions.DatabaseType);
            if (builderCreator == null)
            {
                throw new TuanException($"Unable to resolve {typeof(IDbContextOptionsBuilderCreator).FullName} instance of type '{resolveOptions.DatabaseType}'");
            }
            DbContextOptionsBuilder optionsBuilder = builderCreator.Create(resolveOptions.ConnectionString, resolveOptions.ExistingConnection);
            DbContextModelCache modelCache = _serviceProvider.GetService<DbContextModelCache>();
            var model = modelCache.Get(dbContextType);
            if (model != null)
            {
                optionsBuilder.UseModel(model);
            }
            DbContextOptions options = optionsBuilder.Options;

            if (!(ActivatorUtilities.CreateInstance(_serviceProvider, dbContextType, options) is DbContext context))
            {
                throw new TuanException($"Instantiating the data context '{dbContextType.AssemblyQualifiedName}' failed");
            }
            return context as IDbContext;
        }
    }
}