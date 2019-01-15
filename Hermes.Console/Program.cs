using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdobeMessageInterface;
using DocumentGenerationMessage.Adobe;
using EnvelopeMessage;
using EnvelopeMessage.Base;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Persistence.Sql;

namespace Hermes.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfiguration = new EndpointConfiguration("ConsoleApp");
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            var transportExtensions = endpointConfiguration.UseTransport<MsmqTransport>();
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
                    {
                        DataSource = "localhost",
                        InitialCatalog = "nServiceBus",
                        IntegratedSecurity = true,
                        MultipleActiveResultSets = true
                    };
                    return new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
                });
            endpointConfiguration.EnableInstallers();

            IEndpointInstance endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(true);

            IEnvelopeMessage envelopeMessage = new EnvelopeMessageBase()
            {
                Environment = "DEV",
                From = endpointConfiguration.GetSettings().EndpointName(),
                To = "Hermes.EndpointLoadBalancer.Service",
                Version = "1.0.0",
                Message = typeof(DocumentGenerationMessageAdobe).Name
            };

            await endpoint.Send("Hermes.EndpointLoadBalancer.Service", envelopeMessage);

        }
    }
}
