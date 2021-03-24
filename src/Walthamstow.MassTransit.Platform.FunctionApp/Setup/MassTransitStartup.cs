using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.WebJobs.ServiceBusIntegration;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Serilog;
using Walthamstow.MassTransit.Platform.FunctionApp.Configurations;
using Walthamstow.MassTransit.Platform.FunctionApp.Setup.Implementations;
using Walthamstow.MassTransit.Platform.FunctionApp.Setup.Interfaces;

namespace Walthamstow.MassTransit.Platform.FunctionApp.Setup
{
    public class MassTransitStartup
    {
        public void Configure(IFunctionsHostBuilder builder)
        {
            Configuration = ServiceConfigurationReader.CreateConfiguration();
            var serviceCollection = builder.Services;
            ConfigureServices(serviceCollection);
        }

        public IConfiguration Configuration { get; private set; }

        private void ConfigureServices(IServiceCollection services)
        {
           Log.Information("Configuring MassTransit Services");
 
            services.Configure<PlatformOptions>(Configuration.GetSection("Platform"));

            ServiceBusStartupBusFactory.Configure(services, Configuration);

            var configurationServiceProvider = services.BuildServiceProvider();
            
            services.ConfigureSagaDbs(Configuration);
            
            List<IPlatformStartup> platformStartups = configurationServiceProvider.
                GetService<IEnumerable<IPlatformStartup>>()?.ToList();

            ConfigureApplicationInsights(services);

            services
                .AddSingleton<IMessageReceiver, MessageReceiver>()
                .AddSingleton<IAsyncBusHandle, AsyncBusHandle>();
            
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(cfg =>
            {
                foreach (var platformStartup in platformStartups)
                    platformStartup.ConfigurePlatform(cfg, services, Configuration);

                CreateBus(cfg, configurationServiceProvider);
            });
 
        }

        void ConfigureApplicationInsights(IServiceCollection services)
        {
            if (string.IsNullOrWhiteSpace(Configuration.GetSection("ApplicationInsights")?.GetValue<string>("InstrumentationKey")))
                return;

            Log.Information("Configuring Application Insights");
    
            services.AddApplicationInsightsTelemetry();

            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            {
                module.IncludeDiagnosticSourceActivities.Add("MassTransit");
            });
        }

        void CreateBus(IServiceCollectionBusConfigurator busConfigurator, IServiceProvider provider)
        {
            var platformOptions = provider.GetRequiredService<IOptions<PlatformOptions>>().Value;
            var configurator = new StartupBusConfigurator(platformOptions);
            switch (platformOptions.Transport.ToLower(CultureInfo.InvariantCulture))
            {
                case PlatformOptions.AzureServiceBus:
                case PlatformOptions.ASB:
                    new ServiceBusStartupBusFactory().CreateBus(busConfigurator, configurator);
                    break;
                
                case PlatformOptions.Mediator:
                    break;
                default:
                    throw new ConfigurationException($"Unknown transport type: {platformOptions.Transport}");
            }
        }
    }
}