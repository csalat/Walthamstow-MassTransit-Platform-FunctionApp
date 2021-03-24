using MassTransit;

namespace Walthamstow.MassTransit.Platform.FunctionApp.Setup.Interfaces
{
    public interface IStartupBusConfigurator
    {
        bool HasSchedulerEndpoint { get; }

        void ConfigureBus<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, IBusRegistrationContext context)
            where TEndpointConfigurator : IReceiveEndpointConfigurator;

        bool TryConfigureQuartz(IBusFactoryConfigurator configurator);
    }
}