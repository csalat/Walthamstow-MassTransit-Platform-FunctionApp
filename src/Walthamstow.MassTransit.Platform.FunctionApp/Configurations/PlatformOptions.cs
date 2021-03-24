using System;
using MassTransit.Azure.ServiceBus.Core.Topology;

namespace Walthamstow.MassTransit.Platform.FunctionApp.Configurations
{
    public class PlatformOptions
    {
        public const string AzureServiceBus = "servicebus";
        public const string ASB = "asb";

        public const string Mediator = "mediator";

        public PlatformOptions()
        {
            Transport = Mediator;
        }

        public string Transport { get; set; }


        /// <summary>
        /// If specified, is the queue name of the endpoint where the message scheduler is running (if using Quartz or HangFire)
        /// </summary>
        public string Scheduler { get; set; }

        public bool TryGetSchedulerEndpointAddress(out Uri address)
        {
            if (!string.IsNullOrWhiteSpace(Scheduler))
            {
                if (Uri.IsWellFormedUriString(Scheduler, UriKind.Absolute))
                {
                    address = new Uri(Scheduler);
                    return true;
                }

                switch (Transport.ToLowerInvariant())
                {
                    case AzureServiceBus when ServiceBusEntityNameValidator.Validator.IsValidEntityName(Scheduler):
                    case ASB when ServiceBusEntityNameValidator.Validator.IsValidEntityName(Scheduler):
                        address = new Uri($"queue:{Scheduler}");
                        return true;

                    case Mediator:
                        address = default;
                        return true;
                }
            }

            address = default;
            return false;
        }
    }
}