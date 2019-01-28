using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using Hermes.Common.Datatypes;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;

namespace Hermes.EndpointWorker.Service
{
    class Host
    {
        static readonly ILog log = LogManager.GetLogger<Host>();

        IEndpointInstance _endpoint;

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
                                DataSource = "localhost",
                                InitialCatalog = "nServiceBus",
                                IntegratedSecurity = true,
                                MultipleActiveResultSets = true
                            };
                            return new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
                        });
                }

                endpointConfiguration.EnableInstallers();

                _endpoint = await Endpoint.Start(endpointConfiguration);

                RegisterEndpoint();

                // Set up a timer to trigger every minute.  
                System.Timers.Timer timer = new System.Timers.Timer { Interval = 5000 };
                timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
                timer.Start();

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
                // TODO: perform any further shutdown operations before or after stopping the endpoint
                UnRegisterEndpoint();

                await _endpoint?.Stop();
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

        private void RegisterEndpoint()
        {
            log.Debug("Begin RegisterEndpoint");

            var interfaceGenericArgumentNames = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(t => t.Namespace != null && (!t.Namespace.StartsWith("NServiceBus") && IsMessageHandler(t))).ToList();

            interfaceGenericArgumentNames?.ForEach(interfaceGenericArgumentName =>
            {
                log.Debug(interfaceGenericArgumentName.FullName);
            });

            EndpointRegistration endpointRegistration = new EndpointRegistration()
            {
                EndpointName = $"{EndpointName}@{Environment.MachineName}",
                Environment = "*",
                Message = String.Join(", ", interfaceGenericArgumentNames),
                Version = "1.0.0.0",
                UtcTimestamp = DateTime.UtcNow, 
                RefreshIntervalInSeconds = 5
            };

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("Content-Type", "application/json");
                    webClient.UploadString("http://localhost:9000/api/Config/RegisterEndpoint", "POST", JsonConvert.SerializeObject(endpointRegistration));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            log.Debug("End RegisterEndpoint");
        }

        private void UpdateEndpointRegistration()
        {
            log.Debug("Begin UpdateEndpointRegistration");

            var interfaceGenericArgumentNames = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(t => t.Namespace != null && (!t.Namespace.StartsWith("NServiceBus") && IsMessageHandler(t))).Select(a => a.GetInterface(IHandleMessagesType.Name).GenericTypeArguments[0].Name).ToList();

            interfaceGenericArgumentNames?.ForEach(interfaceGenericArgumentName =>
            {
                log.Debug(interfaceGenericArgumentName);
            });

            EndpointRegistration endpointRegistration = new EndpointRegistration()
            {
                EndpointName = $"{EndpointName}@{Environment.MachineName}",
                Environment = "*",
                Message = String.Join(", ", interfaceGenericArgumentNames),
                Version = "1.0.0.0",
                UtcTimestamp = DateTime.UtcNow
            };

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("Content-Type", "application/json");
                    webClient.UploadString("http://localhost:9000/api/Config/UpdateEndpointRegistration", "PUT", JsonConvert.SerializeObject(endpointRegistration));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            log.Debug("End UpdateEndpointRegistration");
        }

        private void UnRegisterEndpoint()
        {
            log.Debug("Begin UnRegisterEndpoint");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(t => t.Namespace != null && (!t.Namespace.StartsWith("NServiceBus") && IsMessageHandler(t))).ToList();

            assemblies?.ForEach(a =>
            {
                log.Debug(a.FullName);
            });

            EndpointRegistration endpointRegistration = new EndpointRegistration()
            {
                EndpointName = $"{EndpointName}@{Environment.MachineName}",
                Environment = "*",
                Message = String.Join(", ", assemblies),
                Version = "1.0.0.0"
            };

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("Content-Type", "application/json");
                    webClient.UploadString("http://localhost:9000/api/Config/UnRegisterEndpoint", "DELETE", JsonConvert.SerializeObject(endpointRegistration));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            log.Debug("End UnRegisterEndpoint");
        }

        private static bool IsMessageHandler(Type type)
        {
            if (type.IsAbstract || type.IsGenericTypeDefinition)
            {
                return false;
            }

            return type.GetInterfaces().Where(@interface => @interface.IsGenericType).Select(@interface => @interface.GetGenericTypeDefinition()).Any(genericTypeDef => genericTypeDef == IHandleMessagesType);
        }

        static readonly Type IHandleMessagesType = typeof(IHandleMessages<>);

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {

            log.Debug("Begin OnTimer");

            UpdateEndpointRegistration();

            log.Debug("End OnTimer");

        }
    }
}
