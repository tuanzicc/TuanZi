using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.AspNetCore;
using TuanZi.Core.Packs;
using TuanZi.Exceptions;
using TuanZi.Extensions;

using Swashbuckle.AspNetCore.Swagger;


namespace TuanZi.Swagger
{
    [DependsOnPacks(typeof(AspNetCorePack))]
    public class SwaggerPack : AspTuanPack
    {
        private string _title, _url;
        private int _version;

        public override PackLevel Level => PackLevel.Application;

        public override int Order => 2;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            IConfiguration configuration = services.GetConfiguration();
            _url = configuration["Tuan:Swagger:Url"];
            if (_url.IsNullOrEmpty())
            {
                throw new TuanException("The Url of the Swagger node in the configuration file cannot be empty.");
            }

            _title = configuration["Tuan:Swagger:Title"];
            _version = configuration["Tuan:Swagger:Version"].CastTo(1);
            bool enabled = configuration["Tuan:Swagger:Enabled"].CastTo(false);

            if (enabled)
            {
                services.AddMvcCore().AddApiExplorer();
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc($"v{_version}", new Info() { Title = _title, Version = $"v{_version}" });
                    Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml").ToList().ForEach(file =>
                    {
                        options.IncludeXmlComments(file);
                    });
                    options.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                    {
                        Description = "Please enter a Token with Bearer in the form of 'Bearer {Token}'",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                    });
                    options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>()
                    {
                        { "Bearer", Enumerable.Empty<string>() }
                    });
                });
            }
            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            IConfiguration configuration = app.ApplicationServices.GetService<IConfiguration>();
            bool enabled = configuration["Tuan:Swagger:Enabled"].CastTo(false);
            bool miniProfilerEnabled = configuration["Tuan:Swagger:MiniProfiler"].CastTo(false);
            if (enabled)
            {
                app.UseSwagger().UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(_url, $"{_title} V{_version}");
                    if (miniProfilerEnabled)
                    {
                        options.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("TuanZi.Swagger.index.html"); 
                    }
                });
                IsEnabled = true;
            }
        }
    }
}