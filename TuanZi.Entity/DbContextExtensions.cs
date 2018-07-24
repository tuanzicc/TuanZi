using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using TuanZi.Audits;
using TuanZi.Collections;
using TuanZi.Core;
using TuanZi.Core.EntityInfos;
using TuanZi.Dependency;
using TuanZi.Exceptions;

namespace TuanZi.Entity
{
    public static class DbContextExtensions
    {
        public static bool IsRelationalTransaction(this DbContext context)
        {
            return context.Database.GetService<IDbContextTransactionManager>() is IRelationalTransactionManager;
        }

        public static bool ExistsRelationalDatabase(this DbContext context)
        {
            return context.Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator creator && creator.Exists();
        }

        public static void CheckAndMigration(this DbContext dbContext)
        {
            string[] migrations = dbContext.Database.GetPendingMigrations().ToArray();
            if (migrations.Length > 0)
            {
                dbContext.Database.Migrate();
                ILogger logger = ServiceLocator.Instance.GetLogger("TuanZi.Entity.DbContextExtensions");
                logger.LogInformation($"A pending migration record for {migrations.Length} has been submitted:{migrations.ExpandAndToString()}");
            }
        }

        public static int ExecuteSqlCommand(this IDbContext dbContext, string sql, params object[] parameters)
        {
            if (!(dbContext is DbContext context))
            {
                throw new TuanException($"The parameter dbContext is type of '{dbContext.GetType()}' and cannot be converted to DbContext");
            }
            return context.Database.ExecuteSqlCommand(new RawSqlString(sql), parameters);
        }

        public static Task<int> ExecuteSqlCommandAsync(this IDbContext dbContext, string sql, params object[] parameters)
        {
            if (!(dbContext is DbContext context))
            {
                throw new TuanException($"The parameter dbContext is type of '{dbContext.GetType()}' and cannot be converted to DbContext");
            }
            return context.Database.ExecuteSqlCommandAsync(new RawSqlString(sql), parameters);
        }

        public static IList<AuditEntity> GetAuditEntities(this DbContext context)
        {
            List<AuditEntity> result = new List<AuditEntity>();
            IEntityInfoHandler entityInfoHandler = ServiceLocator.Instance.GetService<IEntityInfoHandler>();
            if (entityInfoHandler == null)
            {
                return result;
            }
            EntityState[] states = { EntityState.Added, EntityState.Modified, EntityState.Deleted };
            List<EntityEntry> entries = context.ChangeTracker.Entries().Where(m => m.Entity != null && states.Contains(m.State)).ToList();
            if (entries.Count == 0)
            {
                return result;
            }
            foreach (EntityEntry entry in entries)
            {
                IEntityInfo entityInfo = entityInfoHandler.GetEntityInfo(entry.Entity.GetType());
                if (entityInfo == null || !entityInfo.AuditEnabled)
                {
                    continue;
                }
                result.Add(GetAuditEntity(entry, entityInfo));
            }
            return result;
        }

        private static AuditEntity GetAuditEntity(EntityEntry entry, IEntityInfo entityInfo)
        {
            AuditEntity audit = new AuditEntity() { Name = entityInfo.Name, TypeName = entityInfo.TypeName, OperateType = OperateType.Insert };
            EntityProperty[] entityProperties = entityInfo.Properties;
            foreach (IProperty property in entry.CurrentValues.Properties)
            {
                if (property.IsConcurrencyToken)
                {
                    continue;
                }
                string name = property.Name;
                if (property.IsPrimaryKey())
                {
                    audit.EntityKey = entry.State == EntityState.Deleted
                        ? entry.Property(property.Name).OriginalValue?.ToString()
                        : entry.Property(property.Name).CurrentValue?.ToString();
                }
                AuditEntityProperty auditProperty = new AuditEntityProperty()
                {
                    FieldName = name,
                    DisplayName = entityProperties.First(m => m.Name == name).Display,
                    DataType = property.ClrType.ToString()
                };
                if (entry.State == EntityState.Added)
                {
                    auditProperty.NewValue = entry.Property(property.Name).CurrentValue?.ToString();
                }
                else if (entry.State == EntityState.Deleted)
                {
                    auditProperty.OriginalValue = entry.Property(property.Name).OriginalValue?.ToString();
                }
                else if (entry.State == EntityState.Modified)
                {
                    string currentValue = entry.Property(property.Name).CurrentValue?.ToString();
                    string originalValue = entry.Property(property.Name).OriginalValue?.ToString();
                    if (currentValue != originalValue)
                    {
                        auditProperty.NewValue = currentValue;
                        auditProperty.OriginalValue = originalValue;
                    }
                }
                audit.Properties.Add(auditProperty);
            }
            return audit;
        }
    }
}