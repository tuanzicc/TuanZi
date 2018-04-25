using System;
using System.Collections;
using System.Collections.Generic;


namespace TuanZi.Audits
{
    public class AuditOperation
    {
        public AuditOperation()
        {
            AuditEntities = new List<AuditEntity>();
        }

        public string FunctionName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Ip { get; set; }

        public string UserAgent { get; set; }

        public DateTime CreatedTime { get; set; }

        public ICollection<AuditEntity> AuditEntities { get; set; }
    }
}