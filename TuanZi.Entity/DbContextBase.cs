using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TuanZi.Audits;
using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.Entity.Transactions;
using TuanZi.EventBuses;


namespace TuanZi.Entity
{

    public abstract class DbContextBase : DbContext, IDbContext
    {
        private readonly ILogger _logger;
        private readonly TuanDbContextOptions _TuanDbOptions;
        private readonly IEntityConfigurationTypeFinder _typeFinder;

        protected DbContextBase(DbContextOptions options, IEntityConfigurationTypeFinder typeFinder)
            : base(options)
        {
            _typeFinder = typeFinder;
            IOptions<TuanOptions> TuanOptions = this.GetService<IOptions<TuanOptions>>();
            _TuanDbOptions = TuanOptions?.Value.DbContexts.Values.FirstOrDefault(m => m.DbContextType == GetType());
            _logger = this.GetService<ILoggerFactory>().CreateLogger(GetType());
        }

        public UnitOfWork UnitOfWork { get; set; }

        public void BeginOrUseTransaction()
        {
            if (UnitOfWork == null)
            {
                return;
            }
            UnitOfWork.BeginOrUseTransaction();
        }

        public async Task BeginOrUseTransactionAsync(CancellationToken cancellationToken)
        {
            if (UnitOfWork == null)
            {
                return;
            }
            await UnitOfWork.BeginOrUseTransactionAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Type contextType = GetType();
            IEntityRegister[] registers = _typeFinder.GetEntityRegisters(contextType);
            foreach (IEntityRegister register in registers)
            {
                register.RegistTo(modelBuilder);
                _logger?.LogDebug($"Register the entity class '{register.EntityType}' in the context '{contextType}'");
            }
            _logger?.LogInformation($"Context '{contextType}' registered {registers.Length} entity classes");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_TuanDbOptions != null && _TuanDbOptions.LazyLoadingProxiesEnabled)
            {
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        public override int SaveChanges()
        {
            IList<AuditEntityEntry> auditEntities = new List<AuditEntityEntry>();
            if (_TuanDbOptions != null && _TuanDbOptions.AuditEntityEnabled)
            {
                auditEntities = this.GetAuditEntities();
            }
            BeginOrUseTransaction();

            int count = base.SaveChanges();
            if (count > 0 && auditEntities.Count > 0)
            {
                AuditEntityEventData eventData = new AuditEntityEventData(auditEntities);
                IEventBus eventBus = this.GetService<IEventBus>();
                eventBus.Publish(this, eventData);
            }
            return count;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            IList<AuditEntityEntry> auditEntities = new List<AuditEntityEntry>();
            if (_TuanDbOptions != null && _TuanDbOptions.AuditEntityEnabled)
            {
                auditEntities = this.GetAuditEntities();
            }

            await BeginOrUseTransactionAsync(cancellationToken);

            int count = await base.SaveChangesAsync(cancellationToken);
            if (count > 0 && auditEntities.Count > 0)
            {
                AuditEntityEventData eventData = new AuditEntityEventData(auditEntities);
                IEventBus eventBus = this.GetService<IEventBus>();
                await eventBus.PublishAsync(this, eventData);
            }
            return count;
        }

        #region Overrides of DbContext

        public override void Dispose()
        {
            base.Dispose();
            UnitOfWork = null;
        }

        #endregion
    }
}