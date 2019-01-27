using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvelopeMessage;
using EnvelopeMessage.Base;
using NServiceBus;

namespace DocumentGenerationMessage.Adobe
{
    public class DocumentGenerationMessageAdobe : EnvelopeMessageBase, IDocumentGenerationMessage
    {
        public string DocumentName { get; set; }
    }
}
