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
            DbContexts = new ConcurrentDictionary<string, TuanDbContextOptions>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, TuanDbContextOptions> DbContexts { get; }

        public MailSenderOptions MailSender { get; set; }

        public JwtOptions Jwt { get; set; }

        public RedisOptions Redis { get; set; }

        public SwaggerOptions Swagger { get; set; }

        public TuanDbContextOptions GetDbContextOptions(Type dbContextType)
        {
            return DbContexts.Values.SingleOrDefault(m => m.DbContextType == dbContextType);
        }
    }
}