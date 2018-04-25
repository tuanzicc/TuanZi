using System;
using System.ComponentModel;

using TuanZi.Entity;


namespace TuanZi.Core.Functions
{
    public abstract class FunctionBase : EntityBase<Guid>, IFunction
    {
        public string Name { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public bool IsController { get; set; }

        public bool IsAjax { get; set; }

        public FunctionAccessType AccessType { get; set; }

        public bool IsAccessTypeChanged { get; set; }

        public bool AuditOperationEnabled { get; set; }

        public bool AuditEntityEnabled { get; set; }

        public int CacheExpirationSeconds { get; set; }

        public bool IsCacheSliding { get; set; }

        public bool IsLocked { get; set; }
    }
}