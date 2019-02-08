using System.Collections.Generic;
using System.Data;

using Dapper;

using Microsoft.EntityFrameworkCore;

namespace TuanZi.Entity
{
    public abstract class SqlExecutorBase<TEntity, TKey> : ISqlExecutor<TEntity, TKey> where TEntity : IEntity<TKey>
    {
        private readonly string _connectionString;

        protected SqlExecutorBase(IUnitOfWorkManager unitOfWorkManager)
        {
            DbContext dbContext = (DbContext)unitOfWorkManager.GetDbContext<TEntity, TKey>();
            _connectionString = dbContext.Database.GetDbConnection().ConnectionString;
        }

        public abstract DatabaseType DatabaseType { get; }

        protected abstract IDbConnection GetDbConnection(string connectionString);

        public virtual IEnumerable<TResult> FromSql<TResult>(string sql, object param = null)
        {
            using (IDbConnection db = GetDbConnection(_connectionString))
            {
                return db.Query<TResult>(sql, param);
            }
        }

        public virtual int ExecuteSqlCommand(string sql, object param = null)
        {
            using (IDbConnection db = GetDbConnection(_connectionString))
            {
                return db.Execute(sql, param);
            }
        }
    }
}