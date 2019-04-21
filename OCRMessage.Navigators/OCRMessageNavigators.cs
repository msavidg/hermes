using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCRMessageInterface;

namespace OCRMessage.Navigators
{
    public class OCRMessageNavigators : EnvelopeMessage.EnvelopeMessage, IOCRMessage
    {
        public int DocumentId { get; set; }

    }
}
