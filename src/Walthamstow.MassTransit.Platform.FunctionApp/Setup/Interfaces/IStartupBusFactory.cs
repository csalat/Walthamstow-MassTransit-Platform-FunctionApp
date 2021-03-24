using MassTransit.ExtensionsDependencyInjectionIntegration;

namespace Walthamstow.MassTransit.Platform.FunctionApp.Setup.Interfaces
{
    public interface IStartupBusFactory
    {
        void CreateBus(IServiceCollectionBusConfigurator busConfigurator, IStartupBusConfigurator configurator);
    }
}