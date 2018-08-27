using System;

using TuanZi.Entity;


namespace TuanZi.Core.Options
{
    public class TuanDbContextOptions
    {
        public TuanDbContextOptions()
        {
            LazyLoadingProxiesEnabled = false;
            AuditEntityEnabled = false;
            AutoMigrationEnabled = false;
        }

        public Type DbContextType => string.IsNullOrEmpty(DbContextTypeName) ? null : Type.GetType(DbContextTypeName);

        public string DbContextTypeName { get; set; }

        public string ConnectionString { get; set; }

        public DatabaseType DatabaseType { get; set; }

        public bool LazyLoadingProxiesEnabled { get; set; }

        public bool AuditEntityEnabled { get; set; }

        public bool AutoMigrationEnabled { get; set; }
    }
}