using System.Data.Common;

using Microsoft.EntityFrameworkCore;


namespace TuanZi.Entity
{
    public interface IDbContextOptionsBuilderCreator
    {
        DatabaseType Type { get; }

        DbContextOptionsBuilder Create(string connectionString, DbConnection existingConnection);
    }
}