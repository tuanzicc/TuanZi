using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using TuanZi.Entity;


namespace TuanZi.Core.Options
{
    public class TuanOptions
    {
        public TuanOptions()
        {
            DbContextOptionses = new ConcurrentDictionary<string, TuanDbContextOptions>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, TuanDbContextOptions> DbContextOptionses { get; }

        public MailSenderOptions MailSender { get; set; }

        public TuanDbContextOptions GetDbContextOptions(Type dbContextType)
        {
            return DbContextOptionses.Values.SingleOrDefault(m => m.DbContextType == dbContextType);
        }
    }
}