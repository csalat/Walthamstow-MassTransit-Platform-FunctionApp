using MassTransit.ExtensionsDependencyInjectionIntegration;

namespace Walthamstow.MassTransit.Platform.FunctionApp.Startup
{
    public interface IStartupBusFactory
    {
        void CreateBus(IServiceCollectionBusConfigurator busConfigurator, IStartupBusConfigurator configurator);
    }
}