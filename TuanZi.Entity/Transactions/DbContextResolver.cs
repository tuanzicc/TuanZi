using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Dependency;
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
        
        public DbContext Resolve(DbContextResolveOptions resolveOptions)
        {
            Type dbContextType = resolveOptions.DbContextType;
            IDbContextOptionsBuilderCreator builderCreator = _serviceProvider.GetServices<IDbContextOptionsBuilderCreator>()
                .FirstOrDefault(m => m.Type == resolveOptions.DatabaseType);
            if (builderCreator == null)
            {
                throw new TuanException($"Cannot resolve {typeof(IDbContextOptionsBuilderCreator).FullName} instance of type '{resolveOptions.DatabaseType}'");
            }
            DbContextOptions options = builderCreator.Create(resolveOptions.ConnectionString, resolveOptions.ExistingConnection).Options;

            if (!(ActivatorUtilities.CreateInstance(_serviceProvider, dbContextType, options) is DbContext context))
            {
                throw new TuanException($"Instantiated data context '{dbContextType.AssemblyQualifiedName}' failed");
            }
            return context;
        }
    }
}