using System;

using TuanZi.Entity;


namespace TuanZi.Security
{
    public abstract class EntityInfoInputDtoBase : IInputDto<Guid>
    {
        public bool AuditEnabled { get; set; }

        public Guid Id { get; set; }
    }
}