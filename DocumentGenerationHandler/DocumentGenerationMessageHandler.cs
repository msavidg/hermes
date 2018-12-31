using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdobeMessageInterface;
using NServiceBus;
using NServiceBus.Logging;

namespace DocumentGenerationHandler
{
    public class DocumentGenerationMessageHandler : IHandleMessages<IDocumentGenerationMessage>
    {

        static ILog log = LogManager.GetLogger<IDocumentGenerationMessage>();

        public Task Handle(IDocumentGenerationMessage message, IMessageHandlerContext context)
        {
            log.Debug($"DocumentName: {message.DocumentName}");
            return Task.CompletedTask;
        }
    }
}
