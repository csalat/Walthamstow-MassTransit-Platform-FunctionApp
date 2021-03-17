using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Walthamstow.MassTransit.Platform.FunctionApp.SagaConfig;

namespace Walthamstow.MassTransit.Platform.FunctionApp.Startup
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