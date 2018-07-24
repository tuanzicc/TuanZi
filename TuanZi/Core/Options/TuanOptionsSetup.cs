using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using TuanZi.Entity;
using TuanZi.Exceptions;


namespace TuanZi.Core.Options
{
    public class TuanOptionsSetup : IConfigureOptions<TuanOptions>
    {
        private readonly IConfiguration _configuration;

        public TuanOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(TuanOptions options)
        {
            SetDbContextOptionses(options);

            IConfigurationSection section = _configuration.GetSection("Tuan:MailSender");
            MailSenderOptions sender = section.Get<MailSenderOptions>();
            if (sender != null)
            {
                if (sender.Password == null)
                {
                    sender.Password = _configuration["Tuan:MailSender:Password"];
                }
                options.MailSender = sender;
            }

            section = _configuration.GetSection("Tuan:Jwt");
            JwtOptions jwt = section.Get<JwtOptions>();
            if (jwt != null)
            {
                if (jwt.Secret == null)
                {
                    jwt.Secret = _configuration["Tuan:Jwt:Secret"];
                }
                options.Jwt = jwt;
            }
        }

        private void SetDbContextOptionses(TuanOptions options)
        {
            IConfigurationSection section = _configuration.GetSection("Tuan:DbContexts");
            IDictionary<string, TuanDbContextOptions> dict = section.Get<Dictionary<string, TuanDbContextOptions>>();
            if (dict == null || dict.Count == 0)
            {
                string connectionString = _configuration["ConnectionStrings:DefaultDbContext"];
                if (connectionString == null)
                {
                    return;
                }
                TuanDbContextOptions dbContextOptions = new TuanDbContextOptions()
                {
                    DbContextTypeName = "Tuan.Entity.DefaultDbContext,Tuan.EntityFrameworkCore",
                    ConnectionString = connectionString,
                    DatabaseType = DatabaseType.SqlServer
                };
                options.DbContextOptionses.Add("DefaultDbContext", dbContextOptions);
                return;
            }
            var repeated = dict.Values.GroupBy(m => m.DbContextType).FirstOrDefault(m => m.Count() > 1);
            if (repeated != null)
            {
                throw new TuanException($"Multiple configuration nodes in the data context configuration point to the same context type: {repeated.First().DbContextTypeName}");
            }

            foreach (KeyValuePair<string, TuanDbContextOptions> pair in dict)
            {
                options.DbContextOptionses.Add(pair.Key, pair.Value);
            }
        }
    }
}