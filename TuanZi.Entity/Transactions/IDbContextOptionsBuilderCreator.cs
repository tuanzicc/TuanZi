using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using TuanZi.Dependency;

namespace TuanZi.Entity
{
    [MultipleDependency]
    public interface IDbContextOptionsBuilderCreator
    {
        DatabaseType Type { get; }

        DbContextOptionsBuilder Create(string connectionString, DbConnection existingConnection);
    }
}