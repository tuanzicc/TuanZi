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
                return new DbContextOptionsBuilder().UseSqlServer(connectionString);
            }
            return new DbContextOptionsBuilder().UseSqlServer(existingConnection);
        }
    }
}