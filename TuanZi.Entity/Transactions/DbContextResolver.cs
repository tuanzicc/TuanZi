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
                throw new TuanException($"无法解析类型为“{resolveOptions.DatabaseType}”的 {typeof(IDbContextOptionsBuilderCreator).FullName} 实例");
            }
            DbContextOptions options = builderCreator.Create(resolveOptions.ConnectionString, resolveOptions.ExistingConnection).Options;

            if (!(ActivatorUtilities.CreateInstance(_serviceProvider, dbContextType, options) is DbContext context))
            {
                throw new TuanException($"实例化数据上下文“{dbContextType.AssemblyQualifiedName}”失败");
            }
            return context as IDbContext;
        }
    }
}