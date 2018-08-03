﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Serialization;

using TuanZi.AspNetCore.Mvc.Conventions;
using TuanZi.AspNetCore.Mvc.Filters;
using TuanZi.Core.Packs;
using TuanZi.Net.Email;

namespace TuanZi.AspNetCore.Mvc
{
    public abstract class MvcPackBase : TuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Conventions.Add(new DashedRoutingConvention());
                options.Filters.Add(new FunctionAuthorizationFilter());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDistributedMemoryCache();
            services.AddSingleton<IEmailSender, DefaultEmailSender>();

            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            app.UseMvcWithAreaRoute();
            IsEnabled = true;
        }
    }
}