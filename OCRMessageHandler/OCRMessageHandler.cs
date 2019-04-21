using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NavigatorsOCR.Processing;
using NServiceBus;
using OCRMessageInterface;

namespace OCRMessageHandler
{
    public class OCRMessageHandler : IHandleMessages<IOCRMessage>
    {
        public Task Handle(IOCRMessage message, IMessageHandlerContext context)
        {

            Processing.ProcessDocument(message.DocumentId);

            return Task.CompletedTask;

        }
    }
}
