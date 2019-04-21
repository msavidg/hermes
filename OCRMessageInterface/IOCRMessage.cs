using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvelopeMessageInterface;

namespace OCRMessageInterface
{
    public interface IOCRMessage : IEnvelopeMessage
    {
        int DocumentId { get; set; }
    }
}
