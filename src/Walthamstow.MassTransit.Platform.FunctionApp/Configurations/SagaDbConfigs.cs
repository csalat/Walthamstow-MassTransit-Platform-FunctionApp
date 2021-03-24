using System.Collections.Generic;

namespace Walthamstow.MassTransit.Platform.FunctionApp.Configurations
{
    public class SagaDbConfigs
    {
        public List<SqlServerDbConfigOptions> SagaSqlServerOptions { get; set; }
    }
}