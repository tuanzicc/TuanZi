

using System;

using TuanZi.Core.Functions;
using TuanZi.Entity;


namespace TuanZi.Security
{
    public class FunctionInputDtoBase : IInputDto<Guid>
    {
        public FunctionAccessType AccessType { get; set; }
        
        public bool AuditOperationEnabled { get; set; }
        
        public bool AuditEntityEnabled { get; set; }
        
        public int CacheExpirationSeconds { get; set; }
        
        public bool IsCacheSliding { get; set; }
        
        public bool IsLocked { get; set; }
        
        public Guid Id { get; set; }
    }
}