using EnvelopeMessage;

namespace DocumentGenerationMessage
{
    public interface IDocumentGenerationMessage : IEnvelopeMessage
    {
        string DocumentName { get; set; }
    }
}