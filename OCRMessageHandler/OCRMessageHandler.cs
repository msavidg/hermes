using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using OCRMessageInterface;

namespace OCRMessageHandler
{
    public class OCRMessageHandler : IHandleMessages<IOCRMessage>
    {
        public Task Handle(IOCRMessage message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
