using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DocumentGenerationMessage;
using DocumentGenerationMessage.Adobe;
using Hermes.Common.Datatypes;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Features;
using NServiceBus.Logging;

namespace Hermes.Console
{
    class Program
    {
        static readonly ILog log = LogManager.GetLogger("Console");

        public static string EndpointName => "Hermes.Console";

        private static IEndpointInstance _endpoint;

        static async Task Main(string[] args)
        {
            log.Debug("Begin Main");

            try
            {
                var endpointConfiguration = new EndpointConfiguration("Hermes.Console");

                endpointConfiguration.UseTransport<MsmqTransport>();
                endpointConfiguration.DisableFeature<MessageDrivenSubscriptions>();
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

                endpointConfiguration.SendOnly();

                var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

                IDocumentGenerationMessage documentGenerationMessageAdobe = new DocumentGenerationMessageAdobe()
                {
                    Environment = "DEV",
                    From = endpointConfiguration.GetSettings().EndpointName(),
                    To = "Hermes.EndpointLoadBalancer.Service",
                    Version = "1.0.0",
                    Message = typeof(IDocumentGenerationMessage).Name,
                    DocumentName = "Sample Document"
                };

                await endpointInstance.Send("Hermes.EndpointLoadBalancer.Service", documentGenerationMessageAdobe);

                //RegisterEndpoint();

            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        private static void RegisterEndpoint()
        {
            log.Debug("Begin RegisterEndpoint");

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
