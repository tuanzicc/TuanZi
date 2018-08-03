using System;
using System.Collections.Concurrent;
using System.Security.Claims;

using TuanZi.Audits;
using TuanZi.Core.Functions;


namespace TuanZi.Dependency
{
    public class ScopedDictionary : ConcurrentDictionary<string, object>, IDisposable
    {
        public IFunction Function { get; set; }

        public AuditOperationEntry AuditOperation { get; set; }

        public ClaimsIdentity Identity { get; set; }

        public void Dispose()
        {
            Function = null;
            AuditOperation = null;
            Identity = null;
        }
    }
}