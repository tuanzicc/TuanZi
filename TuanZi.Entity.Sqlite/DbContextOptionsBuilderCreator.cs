
using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using TuanZi.Entity;

namespace TuanZi.Entity.Sqlite
{
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