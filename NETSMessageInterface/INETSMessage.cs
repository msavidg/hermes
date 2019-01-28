using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvelopeMessageInterface;

namespace NETSMessageInterface
{
    public interface INETSMessage : IEnvelopeMessage
    {
        string NETSInfo { get; set; }
    }
}
