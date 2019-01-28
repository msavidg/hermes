using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hermes.Common.Interfaces;

namespace Hermes.Common.Datatypes
{
    public class EndpointRegistration : IEndpointRegistration, IEquatable<EndpointRegistration>
    {
        public string EndpointName { get; set; }
        public string Message { get; set; }
        public string Environment { get; set; }
        public string Version { get; set; }
        public DateTime UtcTimestamp { get; set; }
        public int RefreshIntervalInSeconds { get; set; }


        public EndpointRegistration()
        {
            UtcTimestamp = DateTime.UtcNow;
            RefreshIntervalInSeconds = 5;
        }

        public override string ToString()
        {
            return String.Format($"EndpointName: {this.EndpointName}, Message: {this.Message}, Environment: {this.Environment}, Version: {this.Version}, UtcTimestamp: {this.UtcTimestamp}, RefreshIntervalInSeconds: {this.RefreshIntervalInSeconds}");
        }

        public bool Equals(EndpointRegistration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(EndpointName, other.EndpointName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EndpointRegistration)obj);
        }

        public override int GetHashCode()
        {
            return (EndpointName != null ? EndpointName.GetHashCode() : 0);
        }
    }
}
