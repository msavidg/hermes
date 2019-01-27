using System.Threading.Tasks;
using Hermes.Messages;
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
