using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Hermes.Common.Interfaces;

namespace Hermes.EndpointLoadBalancer.Service.Controllers
{
    [RoutePrefix("api/Config")]
    public class ConfigController : ApiController
    {

        private List<IEndpointRegistration> registrations;

        public ConfigController()
        {
            registrations = new List<IEndpointRegistration>();
        }

        [Route("~/", Name = "default")]
        [Route("GetTime")]
        [HttpGet]
        public string GetTime()
        {
            return DateTime.Now.ToString("O");
        }

        [Route("RegisterEndpoint")]
        [HttpPost]
        public HttpResponseMessage RegisterEndpoint(IEndpointRegistration registration)
        {
            registrations.Add(registration);

            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

            return httpResponseMessage;
        }

        [Route("ShowRegisteredEndpoints")]
        [HttpGet]
        public HttpResponseMessage ShowRegisteredEndpoints()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<!doctype html >");
            stringBuilder.AppendLine("<html>");
            stringBuilder.AppendLine("<table>");
            registrations.ForEach((reg) => { stringBuilder.AppendLine($"<tr><td>{reg.EndpointName}</td></tr>"); });
            stringBuilder.AppendLine("</table>");
            stringBuilder.AppendLine("</html>");

            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent(stringBuilder.ToString(), Encoding.UTF8, "text/html");

            return httpResponseMessage;
        }
    }
}
