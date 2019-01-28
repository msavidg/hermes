using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using EnvelopeMessage;

namespace DocumentGenerationMessage.Adobe
{
    public class DocumentGenerationMessageAdobe : EnvelopeMessage.EnvelopeMessage, IDocumentGenerationMessage
    {
        public string DocumentName { get; set; }
    }
}
