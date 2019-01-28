using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETSMessageInterface;
using NServiceBus;
using NServiceBus.Logging;

namespace NETSMessageHandler
{
    public class NETSMessageHandler : IHandleMessages<INETSMessage>
    {
        static ILog log = LogManager.GetLogger<NETSMessageHandler>();

        public Task Handle(INETSMessage message, IMessageHandlerContext context)
        {
            log.Debug($">>>>> ImageRightFolder: [{message.NETSInfo}]");
            return Task.CompletedTask;
        }
    }
}
