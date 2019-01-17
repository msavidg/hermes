using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;
using Hermes.Common.Interfaces;
using Hermes.EndpointLoadBalancer.Service.Configuration;
using Microsoft.Owin.Hosting;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;
using Owin;

namespace Hermes.EndpointLoadBalancer.Service
{
    class Host
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;
        private const string CacheKey = "endpointRegistrations";

        static readonly ILog log = LogManager.GetLogger<Host>();

        //https://stackoverflow.com/questions/21634333/hosting-webapi-using-owin-in-a-windows-service
        public string baseAddress = "http://+:9000/";
        private IDisposable _server = null;

        IEndpointInstance endpoint;

        public string EndpointName => "Hermes.EndpointLoadBalancer.Service";

        public async Task Start()
        {

            _server = WebApp.Start<OwinStartup>(url: baseAddress);

            CacheItemPolicy policy = new CacheItemPolicy();
            Cache.Set(CacheKey, new List<IEndpointRegistration>(), policy);

            try
            {
                QueueCreationUtils.CreateQueuesForEndpoint(EndpointName, "Everyone");
                QueueCreationUtils.CreateQueue(queueName: "error", account: Environment.UserName);
                QueueCreationUtils.CreateQueue(queueName: "audit", account: Environment.UserName);
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
                                DataSource = "localhost",
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

            if (_server != null)
            {
                _server.Dispose();
            }

            try
            {
                // TODO: perform any further shutdown operations before or after stopping the endpoint
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
