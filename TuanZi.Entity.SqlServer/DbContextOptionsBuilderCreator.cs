using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Dependency;

namespace TuanZi.Entity.SqlServer
{
    [Dependency(ServiceLifetime.Singleton)]
    public class DbContextOptionsBuilderCreator : IDbContextOptionsBuilderCreator
    {
        public DatabaseType Type { get; } = DatabaseType.SqlServer;

        public DbContextOptionsBuilder Create(string connectionString, DbConnection existingConnection)
        {
            if (existingConnection == null)
            {
                DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
                return optionsBuilder.UseSqlServer(connectionString, builder => builder.UseRowNumberForPaging());
            }
            return new DbContextOptionsBuilder().UseSqlServer(existingConnection, builder => builder.UseRowNumberForPaging());
        }
    }
}