
using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Dependency;
using TuanZi.Entity;

namespace TuanZi.Entity.Sqlite
{
    [Dependency(ServiceLifetime.Singleton)]
    public class DbContextOptionsBuilderCreator : IDbContextOptionsBuilderCreator
    {
        public DatabaseType Type { get; } = DatabaseType.Sqlite;

        public DbContextOptionsBuilder Create(string connectionString, DbConnection existingConnection)
        {
            if (existingConnection == null)
            {
                DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
                return optionsBuilder.UseSqlite(connectionString);
            }

            return new DbContextOptionsBuilder().UseSqlite(existingConnection);
        }
    }
}