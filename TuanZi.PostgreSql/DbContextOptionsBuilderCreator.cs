using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Dependency;


namespace TuanZi.Entity.PostgreSql
{
    [Dependency(ServiceLifetime.Singleton)]
    public class DbContextOptionsBuilderCreator : IDbContextOptionsBuilderCreator
    {
        public DatabaseType Type { get; } = DatabaseType.PostgreSql;

        public DbContextOptionsBuilder Create(string connectionString, DbConnection existingConnection)
        {
            if (existingConnection == null)
            {
                return new DbContextOptionsBuilder().UseNpgsql(connectionString);
            }
            return new DbContextOptionsBuilder().UseNpgsql(existingConnection);
        }
    }
}