using Microsoft.Extensions.DependencyInjection;


namespace TuanZi.Dependency
{
    public interface IAppServiceAdder
    {
        IServiceCollection AddServices(IServiceCollection services);
    }
}