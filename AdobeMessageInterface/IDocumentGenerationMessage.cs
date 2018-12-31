using EnvelopeMessage;
using NServiceBus;

namespace AdobeMessageInterface
{
    public interface IDocumentGenerationMessage : IEnvelopeMessage
    {
        string DocumentName { get; set; }
    }
}