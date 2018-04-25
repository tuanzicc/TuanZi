using System.IO;

using Microsoft.Extensions.Configuration;


namespace TuanZi.Core.Options
{
    public static class AppSettingsManager
    {
        private static IConfiguration _configuration;

        static AppSettingsManager()
        {
            BuildConfiguration();
        }

        private static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").AddJsonFile("appsettings.Development.json");
            _configuration = builder.Build();
        }

        public static string Get(string key)
        {
            return _configuration[key];
        }

        public static T Get<T>(string key)
        {
            string json = Get(key);
            return json.FromJsonString<T>();
        }
    }
}