using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hermes.Common.Interfaces;

namespace Hermes.Common.Datatypes
{
    public class EndpointRegistration : IEndpointRegistration
    {
        public string EndpointName { get; set; }
    }
}
