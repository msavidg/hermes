using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETSMessageInterface;

namespace NETSMessage
{
    public class NETSMessage : EnvelopeMessage.EnvelopeMessage, INETSMessage
    {
        public string NETSInfo { get; set; }
    }
}
