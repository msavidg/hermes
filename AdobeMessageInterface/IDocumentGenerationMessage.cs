using EnvelopeMessageInterface;

namespace DocumentGenerationMessage
{
    public interface IDocumentGenerationMessage : IEnvelopeMessage
    {
        string DocumentName { get; set; }
    }
}