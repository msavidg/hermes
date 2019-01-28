using NServiceBus;

namespace EnvelopeMessageInterface
{
    public interface IEnvelopeMessage : ICommand
    {
        string From { get; set; }
        string To { get; set; }
        string Message { get; set; }
        string Environment { get; set; }
        string Version { get; set; }
    }
}