using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Dependency;

namespace TuanZi.Entity.MySql
{
    [Dependency(ServiceLifetime.Singleton)]
    public class DbContextOptionsBuilderCreator : IDbContextOptionsBuilderCreator
    {
        public DatabaseType Type { get; } = DatabaseType.MySql;

        public DbContextOptionsBuilder Create(string connectionString, DbConnection existingConnection)
        {
            if (existingConnection == null)
            {
                return new DbContextOptionsBuilder().UseMySql(connectionString);
            }
            return new DbContextOptionsBuilder().UseMySql(existingConnection);
        }
    }
}