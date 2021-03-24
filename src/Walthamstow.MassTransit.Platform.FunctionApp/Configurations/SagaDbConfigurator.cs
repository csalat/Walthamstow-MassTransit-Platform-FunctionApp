using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Walthamstow.MassTransit.Platform.FunctionApp.Configurations
{
    public static class SagaDbConfigurator
    {
        public static void ConfigureSagaDbs(this IServiceCollection services, IConfiguration configuration)
        {
            var sagaDbConfigs = new SagaDbConfigs();
            configuration.GetSection(nameof(SagaDbConfigs)).Bind(sagaDbConfigs);
            services.AddSingleton(sp => sagaDbConfigs);
        }
    }
}