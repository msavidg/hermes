using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageRightMessageInterface;
using NServiceBus;
using NServiceBus.Logging;

namespace ImageRightMessageHandler
{
    public class ImageRightMessageHandler : IHandleMessages<IImageRightMessage>
    {
        static ILog log = LogManager.GetLogger<ImageRightMessageHandler>();

        public Task Handle(IImageRightMessage message, IMessageHandlerContext context)
        {
            log.Debug($">>>>> ImageRightFolder: [{message.ImageRightFolder}]");
            return Task.CompletedTask;
        }
    }
}
