using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Dependency;

namespace TuanZi.Entity
{
    [MultipleDependency]
    public interface ISqlExecutor<TEntity, TKey> where TEntity : IEntity<TKey>
    {
        DatabaseType DatabaseType { get; }

        IEnumerable<TResult> FromSql<TResult>(string sql, object param = null);

        int ExecuteSqlCommand(string sql, object param = null);
    }
}
