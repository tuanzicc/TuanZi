using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace TuanZi.Entity
{
    public abstract class DesignTimeDbContextFactoryBase<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        public virtual TDbContext CreateDbContext(string[] args)
        {
            string connString = GetConnectionString();
            IEntityConfigurationTypeFinder typeFinder = GetEntityConfigurationTypeFinder();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder<DefaultDbContext>();
            builder = UseSql(builder, connString);
            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), builder.Options, typeFinder);
        }

        public abstract string GetConnectionString();

        public abstract IEntityConfigurationTypeFinder GetEntityConfigurationTypeFinder();

        public abstract DbContextOptionsBuilder UseSql(DbContextOptionsBuilder builder, string connString);
    }
}