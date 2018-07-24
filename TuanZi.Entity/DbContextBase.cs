using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TuanZi.Audits;
using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.EventBuses;


namespace TuanZi.Entity
{
    
    public abstract class DbContextBase : DbContext, IDbContext
    {
        private readonly ILogger _logger;
        private readonly TuanDbContextOptions _osharpDbOptions;
        private readonly IEntityConfigurationTypeFinder _typeFinder;

        protected DbContextBase(DbContextOptions options, IEntityConfigurationTypeFinder typeFinder)
            : base(options)
        {
            _typeFinder = typeFinder;
            if (ServiceLocator.Instance.IsProviderEnabled)
            {
                IOptions<TuanOptions> osharpOptions = ServiceLocator.Instance.GetService<IOptions<TuanOptions>>();
                _osharpDbOptions = osharpOptions?.Value.DbContextOptionses.Values.FirstOrDefault(m => m.DbContextType == GetType());

                _logger = ServiceLocator.Instance.GetLogger(GetType());
            }
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

        public override int SaveChanges()
        {
            IList<AuditEntity> auditEntities = new List<AuditEntity>();
            if (_osharpDbOptions != null && _osharpDbOptions.AuditEntityEnabled && ServiceLocator.InScoped())
            {
                auditEntities = this.GetAuditEntities();
            }
            int count = base.SaveChanges();
            if (count > 0 && auditEntities.Count > 0 && ServiceLocator.InScoped())
            {
                AuditEntityEventData eventData = new AuditEntityEventData(auditEntities);
                IEventBus eventBus = ServiceLocator.Instance.GetService<IEventBus>();
                eventBus.Publish(this, eventData);
            }
            return count;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            IList<AuditEntity> auditEntities = new List<AuditEntity>();
            if (_osharpDbOptions != null && _osharpDbOptions.AuditEntityEnabled && ServiceLocator.InScoped())
            {
                auditEntities = this.GetAuditEntities();
            }
            int count = await base.SaveChangesAsync(cancellationToken);
            if (count > 0 && auditEntities.Count > 0 && ServiceLocator.InScoped())
            {
                AuditEntityEventData eventData = new AuditEntityEventData(auditEntities);
                IEventBus eventBus = ServiceLocator.Instance.GetService<IEventBus>();
                await eventBus.PublishAsync(this, eventData);
            }
            return count;
        }

    }
}