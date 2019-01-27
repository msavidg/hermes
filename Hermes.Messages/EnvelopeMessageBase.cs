namespace Hermes.Messages
{
    public class EnvelopeMessageBase : IEnvelopeMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string Environment { get; set; }
        public string Version { get; set; }
    }
}
