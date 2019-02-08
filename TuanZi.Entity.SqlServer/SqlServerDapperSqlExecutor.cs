using System.Data;
using System.Data.SqlClient;


namespace TuanZi.Entity.SqlServer
{
    public class SqlServerDapperSqlExecutor<TEntity, TKey> : SqlExecutorBase<TEntity, TKey> where TEntity : IEntity<TKey>
    {
        public SqlServerDapperSqlExecutor(IUnitOfWorkManager unitOfWorkManager)
            : base(unitOfWorkManager)
        { }

        public override DatabaseType DatabaseType => DatabaseType.SqlServer;

        protected override IDbConnection GetDbConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}