using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using EnvelopeMessageInterface;
using Hermes.Common.Interfaces;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;

namespace EnvelopeHandler
{
    public class EnvelopeMessageHandler : IHandleMessages<IEnvelopeMessage>
    {

        private static readonly ObjectCache Cache = MemoryCache.Default;
        private const string CacheKey = "endpointRegistrations";

        static ILog log = LogManager.GetLogger<EnvelopeMessageHandler>();

        public EnvelopeMessageHandler()
        {
            log.Debug("EnvelopeMessageHandler::ctor");
        }

        public Task Handle(IEnvelopeMessage message, IMessageHandlerContext context)
        {
            log.Debug($">>>>> Envrionment: {message.Environment}, From: {message.From}, To: {message.To}, Message: {message.Message}, Version: {message.Version}");

            if (Cache[CacheKey] is List<IEndpointRegistration> endpointRegistrations)
            {
                var endpointRegistration = endpointRegistrations.FirstOrDefault(er1 => er1.Message.Split(",".ToCharArray()).Any<string>(er2 => er2.Equals(message.Message, StringComparison.InvariantCultureIgnoreCase)));

                if (endpointRegistration == null)
                {
                    return Task.FromException(new Exception($"No registered endpoint handles: [{message.Message}]"));
                }

                CallWorkerService(message, endpointRegistration).Wait(new TimeSpan(0, 0, 1, 0));

            }

            return Task.CompletedTask;
        }

        private static async Task CallWorkerService(IEnvelopeMessage message, IEndpointRegistration endpointRegistration)
        {
            var endpointConfiguration = new EndpointConfiguration($"{message.To}");

            endpointConfiguration.UseTransport<MsmqTransport>();
            endpointConfiguration.DisableFeature<MessageDrivenSubscriptions>();
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

            endpointConfiguration.SendOnly();

            message.From = message.To;
            message.To = endpointRegistration.EndpointName;

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            log.Debug($"{message.GetType().Name}");

            await endpointInstance.Send(endpointRegistration.EndpointName, Convert.ChangeType(message, message.GetType()));
        }
    }
}
