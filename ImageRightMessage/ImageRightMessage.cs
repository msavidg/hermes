using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageRightMessageInterface;


namespace ImageRightMessage
{
    public class ImageRightMessage : EnvelopeMessage.EnvelopeMessage, IImageRightMessage
    {
        public string ImageRightFolder { get; set; }
    }
}
