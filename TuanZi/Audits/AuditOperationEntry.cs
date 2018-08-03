using System;
using System.Collections.Generic;

using TuanZi.Data;


namespace TuanZi.Audits
{
    public class AuditOperationEntry
    {
        public AuditOperationEntry()
        {
            EntityEntries = new List<AuditEntityEntry>();
        }

        public string FunctionName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Ip { get; set; }

        public string UserAgent { get; set; }

        public AjaxResultType ResultType { get; set; } = AjaxResultType.Success;

        public string Message { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime EndedTime { get; set; }

        public ICollection<AuditEntityEntry> EntityEntries { get; set; }

    }
}