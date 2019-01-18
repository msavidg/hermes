using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using EnvelopeMessage;
using NServiceBus;
using NServiceBus.Logging;

namespace EnvelopeHandler
{
    public class EnvelopeMessageHandler : IHandleMessages<IEnvelopeMessage>
    {

        private static readonly ObjectCache Cache = MemoryCache.Default;
        private const string CacheKey = "endpointRegistrations";

        static ILog log = LogManager.GetLogger<IEnvelopeMessage>();

        public Task Handle(IEnvelopeMessage message, IMessageHandlerContext context)
        {
            log.Debug($"Envrionment: {message.Environment}, From: {message.From}, To: {message.To}, Message: {message.Message}, Version: {message.Version}");
            return Task.CompletedTask;
        }
    }
}
