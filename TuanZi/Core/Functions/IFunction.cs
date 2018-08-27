using System;

using TuanZi.Entity;


namespace TuanZi.Core.Functions
{
    public interface IFunction : IEntity<Guid>, ILockable, IEntityHash
    {
        string Name { get; set; }

        string Area { get; set; }

        string Controller { get; set; }

        string Action { get; set; }

        bool IsController { get; set; }

        bool IsAjax { get; set; }

        FunctionAccessType AccessType { get; set; }

        bool IsAccessTypeChanged { get; set; }

        bool AuditOperationEnabled { get; set; }

        bool AuditEntityEnabled { get; set; }

        int CacheExpirationSeconds { get; set; }

        bool IsCacheSliding { get; set; }
    }
}