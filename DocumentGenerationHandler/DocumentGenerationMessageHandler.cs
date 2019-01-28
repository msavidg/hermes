using System.Threading.Tasks;
using DocumentGenerationMessage;
using NServiceBus;
using NServiceBus.Logging;

namespace DocumentGenerationHandler
{
    public class DocumentGenerationMessageHandler : IHandleMessages<IDocumentGenerationMessage>
    {

        static ILog log = LogManager.GetLogger<DocumentGenerationMessageHandler>();

        public Task Handle(IDocumentGenerationMessage message, IMessageHandlerContext context)
        {
            log.Debug($">>>>> DocumentName: {message.DocumentName}");
            return Task.CompletedTask;
        }
    }
}
