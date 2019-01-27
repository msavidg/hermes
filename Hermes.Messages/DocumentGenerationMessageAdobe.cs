namespace Hermes.Messages
{
    public class DocumentGenerationMessageAdobe : EnvelopeMessageBase, IDocumentGenerationMessage
    {
        public string DocumentName { get; set; }
    }
}
