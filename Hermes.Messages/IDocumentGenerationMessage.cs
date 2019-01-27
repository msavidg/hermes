namespace Hermes.Messages
{
    public interface IDocumentGenerationMessage : IEnvelopeMessage
    {
        string DocumentName { get; set; }
    }
}