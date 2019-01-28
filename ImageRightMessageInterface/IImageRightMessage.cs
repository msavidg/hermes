using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvelopeMessageInterface;

namespace ImageRightMessageInterface
{
    public interface IImageRightMessage : IEnvelopeMessage
    {
        string ImageRightFolder { get; set; }
    }
}
