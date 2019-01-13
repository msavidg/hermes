using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Hermes.EndpointLoadBalancer.Service.Controllers
{
    [RoutePrefix("api/Config")]
    public class ConfigController : ApiController
    {
        [Route("~/",Name="default")]
        [Route("GetTime")]
        public string GetTime()
        {
            return DateTime.Now.ToString("O");
        }
    }
}
