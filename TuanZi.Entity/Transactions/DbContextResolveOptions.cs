using System;
using System.Data.Common;

using TuanZi.Core.Options;


namespace TuanZi.Entity.Transactions
{
    public class DbContextResolveOptions
    {
        public DbContextResolveOptions()
        { }

        public DbContextResolveOptions(TuanDbContextOptions options)
        {
            DbContextType = options.DbContextType;
            ConnectionString = options.ConnectionString;
            DatabaseType = options.DatabaseType;
        }

        public Type DbContextType { get; set; }

        public string ConnectionString { get; set; }

        public DbConnection ExistingConnection { get; set; }

        public DatabaseType DatabaseType { get; set; }
    }
}