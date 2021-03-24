using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using Serilog;
using Walthamstow.MassTransit.Platform.FunctionApp.Configurations;
using Walthamstow.MassTransit.Platform.FunctionApp.Setup.Interfaces;

namespace Walthamstow.MassTransit.Platform.FunctionApp.Setup.Implementations
{
    public class StartupBusConfigurator :
        IStartupBusConfigurator
    {
        readonly PlatformOptions _platformOptions;
        readonly Uri _schedulerEndpointAddress;

        public StartupBusConfigurator(PlatformOptions platformOptions)
        {
            _platformOptions = platformOptions;

            _platformOptions.TryGetSchedulerEndpointAddress(out _schedulerEndpointAddress);
        }

        public bool HasSchedulerEndpoint => _schedulerEndpointAddress != null;

        public void ConfigureBus<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator,
            IBusRegistrationContext context)
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            configurator.UseHealthCheck(context);


            List<IPlatformStartup> hostingConfigurators = context.GetService<IEnumerable<IPlatformStartup>>()?.ToList();

            foreach (var hostingConfigurator in hostingConfigurators)
                hostingConfigurator.ConfigureBus(configurator, context);

            configurator.ConfigureEndpoints(context);
        }

        public bool TryConfigureQuartz(IBusFactoryConfigurator configurator)
        {
            if (_schedulerEndpointAddress == null)
                return false;

            Log.Information("Configuring Quartz Message Scheduler (endpoint: {QuartzAddress}", _schedulerEndpointAddress);
            configurator.UseMessageScheduler(_schedulerEndpointAddress);
            return true;
        }
    }
}