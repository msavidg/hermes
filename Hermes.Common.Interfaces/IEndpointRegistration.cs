using System;

namespace Hermes.Common.Interfaces
{
    public interface IEndpointRegistration
    {
        string EndpointName { get; set; }
        string Message { get; set; }
        string Environment { get; set; }
        string Version { get; set; }
        DateTime UtcTimestamp { get; set; }

        string ToString();
    }
}