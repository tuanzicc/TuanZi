//TuanZi.Me

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using TuanZi.Audits;
using TuanZi.Core.Options;
using TuanZi.EventBuses;


namespace TuanZi.Entity
{
    public abstract class DbContextBase<TDbContext> : DbContext, IDbContext
    {
        private readonly IEntityConfigurationTypeFinder _typeFinder;
        private readonly TuanDbContextOptions _tuanDbOptions;
        
        protected DbContextBase(DbContextOptions options, IEntityConfigurationTypeFinder typeFinder)
            : base(options)
        {
            _typeFinder = typeFinder;
            IOptionsMonitor<TuanOptions> tuanOptions = ServiceLocator.Instance.GetService<IOptionsMonitor<TuanOptions>>();
            if (tuanOptions != null)
            {
                _tuanDbOptions = tuanOptions.CurrentValue.DbContextOptionses.Values.FirstOrDefault(m => m.DbContextType == typeof(TDbContext));
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IEntityRegister[] registers = _typeFinder.GetEntityRegisters(typeof(TDbContext));
            foreach (IEntityRegister register in registers)
            {
                register.RegistTo(modelBuilder);
            }
        }

        /// <summary>
        ///     Saves all changes made in this context to the database.
        /// </summary>
        /// <remarks>
        ///     This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///     changes to entity instances before saving to the underlying database. This can be disabled via
        ///     <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        /// </remarks>
        /// <returns>
        ///     The number of state entries written to the database.
        /// </returns>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override int SaveChanges()
        {
            IList<AuditEntity> auditEntities = new List<AuditEntity>();
            if (_tuanDbOptions != null && _tuanDbOptions.AuditEntityEnabled)
            {
                auditEntities = this.GetAuditEntities();
            }
            int count = base.SaveChanges();
            if (count > 0 && auditEntities.Count > 0)
            {
                AuditEntityEventData eventData = new AuditEntityEventData(auditEntities);
                IEventBus eventBus = ServiceLocator.Instance.GetService<IEventBus>();
                eventBus.PublishSync(this, eventData);
            }
            return count;
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///     </para>
        /// </remarks>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     number of state entries written to the database.
        /// </returns>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            IList<AuditEntity> auditEntities = new List<AuditEntity>();
            if (_tuanDbOptions != null && _tuanDbOptions.AuditEntityEnabled)
            {
                auditEntities = this.GetAuditEntities();
            }
            int count = await base.SaveChangesAsync(cancellationToken);
            if (count > 0 && auditEntities.Count > 0)
            {
                AuditEntityEventData eventData = new AuditEntityEventData(auditEntities);
                IEventBus eventBus = ServiceLocator.Instance.GetService<IEventBus>();
                await eventBus.PublishAsync(this, eventData);
            }
            return count;
        }
    }

}