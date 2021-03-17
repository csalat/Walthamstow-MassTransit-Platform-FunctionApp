namespace Walthamstow.MassTransit.Platform.FunctionApp.Transports.ServiceBus
{
    public class ServiceBusOptions
    {
        public string ConnectionString { get; set; }
        public bool Enabled { get; set; }
    }

}