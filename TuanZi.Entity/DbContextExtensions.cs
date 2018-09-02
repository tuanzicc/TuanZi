using System;
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
using TuanZi.Core.Functions;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Exceptions;

namespace TuanZi.Entity
{
    public static partial class DbContextExtensions
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

        public static void Update<TEntity, TKey>(this DbContext context, params TEntity[] entities)
            where TEntity : class, IEntity<TKey>
        {
            Check.NotNull(entities, nameof(entities));

            DbSet<TEntity> set = context.Set<TEntity>();
            foreach (TEntity entity in entities)
            {
                try
                {
                    EntityEntry<TEntity> entry = context.Entry(entity);
                    if (entry.State == EntityState.Detached)
                    {
                        set.Attach(entity);
                        entry.State = EntityState.Modified;
                    }
                }
                catch (InvalidOperationException)
                {
                    TEntity oldEntity = set.Find(entity.Id);
                    context.Entry(oldEntity).CurrentValues.SetValues(entity);
                }
            }
        }

        public static IList<AuditEntityEntry> GetAuditEntities(this DbContext context)
        {
            List<AuditEntityEntry> result = new List<AuditEntityEntry>();
            ScopedDictionary scopedDict = ServiceLocator.Instance.GetService<ScopedDictionary>();
            IFunction function = scopedDict?.Function;
            if (function == null || !function.AuditEntityEnabled)
            {
                return result;
            }
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
                result.AddIfNotNull(GetAuditEntity(entry, entityInfo));
            }
            return result;
        }

        private static AuditEntityEntry GetAuditEntity(EntityEntry entry, IEntityInfo entityInfo)
        {
            AuditEntityEntry audit = new AuditEntityEntry
            {
                Name = entityInfo.Name,
                TypeName = entityInfo.TypeName,
                OperateType = entry.State == EntityState.Added
                    ? OperateType.Insert
                    : entry.State == EntityState.Modified
                        ? OperateType.Update
                        : entry.State == EntityState.Deleted
                            ? OperateType.Delete
                            : OperateType.Query,
                Entity = entry.Entity
            };
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
                AuditPropertyEntry auditProperty = new AuditPropertyEntry()
                {
                    FieldName = name,
                    DisplayName = entityProperties.First(m => m.Name == name).Display,
                    DataType = property.ClrType.ToString()
                };
                if (entry.State == EntityState.Added)
                {
                    auditProperty.NewValue = entry.Property(property.Name).CurrentValue?.ToString();
                    audit.PropertyEntries.Add(auditProperty);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    auditProperty.OriginalValue = entry.Property(property.Name).OriginalValue?.ToString();
                    audit.PropertyEntries.Add(auditProperty);
                }
                else if (entry.State == EntityState.Modified)
                {
                    string currentValue = entry.Property(property.Name).CurrentValue?.ToString();
                    string originalValue = entry.Property(property.Name).OriginalValue?.ToString();
                    if (currentValue == originalValue)
                    {
                        continue;
                    }
                    auditProperty.NewValue = currentValue;
                    auditProperty.OriginalValue = originalValue;
                    audit.PropertyEntries.Add(auditProperty);
                }
            }
            return audit.PropertyEntries.Count == 0 ? null : audit;
        }
    }
}