using System.ComponentModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TuanZi.Core.Packs;


namespace TuanZi.CodeGeneration
{
    [Description("代码生成模块")]
    public class CodeGenerationPack : TuanPack
    {
        public override PackLevel Level { get; } = PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<ICodeGenerator, RazorCodeGenerator>();

            return services;
        }
    }
}