using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;

namespace Hermes.EndpointWorker.Service
{
    class Host
    {
        static readonly ILog log = LogManager.GetLogger<Host>();

        IEndpointInstance endpoint;

        public string EndpointName => "Hermes.EndpointWorker.Service";

        public async Task Start()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration(EndpointName);
                endpointConfiguration.SendFailedMessagesTo("error");
                endpointConfiguration.AuditProcessedMessagesTo("audit");
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);
                if (Environment.UserInteractive && Debugger.IsAttached)
                {
                    var transportExtensions = endpointConfiguration.UseTransport<LearningTransport>();
                    transportExtensions.StorageDirectory(@"c:\temp");
                    endpointConfiguration.UsePersistence<InMemoryPersistence>();
                }
                else
                {
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
                                DataSource = "PCDANMSAVIDGE10",
                                InitialCatalog = "nServiceBus",
                                IntegratedSecurity = true,
                                MultipleActiveResultSets = true
                            };
                            return new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
                        });
                }
                endpointConfiguration.EnableInstallers();
                endpoint = await Endpoint.Start(endpointConfiguration);
            }
            catch (Exception ex)
            {
                FailFast("Failed to start.", ex);
            }
        }

        public async Task Stop()
        {
            try
            {
                // TODO: perform any futher shutdown operations before or after stopping the endpoint
                await endpoint?.Stop();
            }
            catch (Exception ex)
            {
                FailFast("Failed to stop correctly.", ex);
            }
        }

        async Task OnCriticalError(ICriticalErrorContext context)
        {
            // TODO: decide if stopping the endpoint and exiting the process is the best response to a critical error
            // https://docs.particular.net/nservicebus/hosting/critical-errors
            // and consider setting up service recovery
            // https://docs.particular.net/nservicebus/hosting/windows-service#installation-restart-recovery
            try
            {
                await context.Stop();
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        void FailFast(string message, Exception exception)
        {
            try
            {
                log.Fatal(message, exception);

                // TODO: when using an external logging framework it is important to flush any pending entries prior to calling FailFast
                // https://docs.particular.net/nservicebus/hosting/critical-errors#when-to-override-the-default-critical-error-action
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }
    }
}
