using System.Data;

using Npgsql;


namespace TuanZi.Entity.PostgreSql
{
    public class PostgreSqlDapperSqlExecutor<TEntity, TKey> : SqlExecutorBase<TEntity, TKey> where TEntity : IEntity<TKey>
    {
        public PostgreSqlDapperSqlExecutor(IUnitOfWorkManager unitOfWorkManager)
            : base(unitOfWorkManager)
        { }

        public override DatabaseType DatabaseType => DatabaseType.PostgreSql;

        protected override IDbConnection GetDbConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}