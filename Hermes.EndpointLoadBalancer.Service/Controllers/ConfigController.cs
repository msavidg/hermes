using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Hermes.EndpointLoadBalancer.Service.Controllers
{
    [RoutePrefix("api/Config")]
    public class ConfigController : ApiController
    {


        public ConfigController()
        {
        }

        [Route("~/", Name = "default")]
        [Route("GetTime")]
        public string GetTime()
        {
            return DateTime.Now.ToString("O");
        }

        [Route("ShowRegisteredEndpoints")]
        public HttpResponseMessage ShowRegisteredEndpoints()
        {
            StringBuilder stringBuilder = new StringBuilder();

            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent(stringBuilder.ToString(), Encoding.UTF8, "text/html");

            return httpResponseMessage;
        }
    }
}
