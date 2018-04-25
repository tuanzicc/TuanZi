using System;


namespace TuanZi.Audits
{
    public class AuditOptions
    {
        public Func<AuditOperation> OperationFactory { get; set; }

        public Func<AuditEntity> DataFactory { get; set; }
    }
}