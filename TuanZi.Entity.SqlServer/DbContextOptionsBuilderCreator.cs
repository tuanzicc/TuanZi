using System.Data.Common;

using Microsoft.EntityFrameworkCore;


namespace TuanZi.Entity.SqlServer
{
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