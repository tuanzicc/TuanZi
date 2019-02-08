using System.Data;

using Microsoft.Data.Sqlite;


namespace TuanZi.Entity.Sqlite
{
    public class SqliteDapperSqlExecutor<TEntity, TKey> : SqlExecutorBase<TEntity, TKey> where TEntity : IEntity<TKey>
    {
        public SqliteDapperSqlExecutor(IUnitOfWorkManager unitOfWorkManager)
            : base(unitOfWorkManager)
        { }
        
        public override DatabaseType DatabaseType => DatabaseType.Sqlite;

        protected override IDbConnection GetDbConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }

}