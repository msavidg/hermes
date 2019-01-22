using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AdobeMessageInterface;
using DocumentGenerationMessage.Adobe;
using EnvelopeMessage;
using EnvelopeMessage.Base;
using Hermes.Common.Datatypes;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Hosting.Helpers;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;

namespace Hermes.Console
{
    class Program
    {
        static readonly ILog log = LogManager.GetLogger<Program>();

        public static string EndpointName => "Hermes.EndpointWorker.Service";

        private static IEndpointInstance _endpoint;

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

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(true);

            RegisterEndpoint();

            IEnvelopeMessage envelopeMessage = new EnvelopeMessageBase()
            {
                Environment = "DEV",
                From = endpointConfiguration.GetSettings().EndpointName(),
                To = "Hermes.EndpointLoadBalancer.Service",
                Version = "1.0.0",
                Message = typeof(DocumentGenerationMessageAdobe).Name
            };

            await _endpoint.Send("Hermes.EndpointLoadBalancer.Service", envelopeMessage);

        }

        private static void RegisterEndpoint()
        {
            log.Debug("Begin RegisterEndpoint");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(t => t.Namespace != null && (!t.Namespace.StartsWith("NServiceBus") && IsMessageHandler(t))).ToList();

            assemblies?.ForEach(a =>
            {
                log.Debug(a.FullName);
            });

            EndpointRegistration endpointRegistration = new EndpointRegistration()
            {
                EndpointName = EndpointName,
                Environment = "*",
                Message = String.Join(", ", assemblies),
                Version = "1.0.0.0"
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

        private static bool IsMessageHandler(Type type)
        {
            if (type.IsAbstract || type.IsGenericTypeDefinition)
            {
                return false;
            }

            return type.GetInterfaces().Where(@interface => @interface.IsGenericType).Select(@interface => @interface.GetGenericTypeDefinition()).Any(genericTypeDef => genericTypeDef == IHandleMessagesType);
        }

        static readonly Type IHandleMessagesType = typeof(IHandleMessages<>);
    }
}
