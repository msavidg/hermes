using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Hermes.Common.Datatypes;
using Hermes.Common.Interfaces;

namespace Hermes.EndpointLoadBalancer.Service.Controllers
{
    [RoutePrefix("api/Config")]
    public class ConfigController : ApiController
    {

        private static readonly ObjectCache Cache = MemoryCache.Default;
        private const string CacheKey = "endpointRegistrations";

        public ConfigController()
        {
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
        public HttpResponseMessage RegisterEndpoint(EndpointRegistration endpointRegistration)
        {

            HttpResponseMessage httpResponseMessage;

            if (Cache[CacheKey] is List<IEndpointRegistration> endpointRegistrations)
            {
                endpointRegistrations.Add(endpointRegistration);
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
                httpResponseMessage.Content = new StringContent("Unable to load endpointRegistration list from memory.", Encoding.UTF8, MediaTypeNames.Text.Plain);
            }

            return httpResponseMessage;
        }

        [Route("UpdateEndpointRegistration")]
        [HttpPut]
        public HttpResponseMessage UpdateEndpointRegistration(EndpointRegistration endpointRegistration)
        {

            HttpResponseMessage httpResponseMessage;

            if (Cache[CacheKey] is List<IEndpointRegistration> endpointRegistrations)
            {
                endpointRegistrations.Remove(endpointRegistration);
                endpointRegistrations.Add(endpointRegistration);
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
                httpResponseMessage.Content = new StringContent("Unable to load endpointRegistration list from memory.", Encoding.UTF8, MediaTypeNames.Text.Plain);
            }

            return httpResponseMessage;
        }

        [Route("UnRegisterEndpoint")]
        [HttpDelete]
        public HttpResponseMessage UnRegisterEndpoint(EndpointRegistration endpointRegistration)
        {

            HttpResponseMessage httpResponseMessage;

            if (Cache[CacheKey] is List<IEndpointRegistration> endpointRegistrations)
            {
                endpointRegistrations.Remove(endpointRegistration);
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
                httpResponseMessage.Content = new StringContent("Unable to load endpointRegistration list from memory.", Encoding.UTF8, MediaTypeNames.Text.Plain);
            }

            return httpResponseMessage;
        }

        [Route("ShowRegisteredEndpoints")]
        [HttpGet]
        public HttpResponseMessage ShowRegisteredEndpoints()
        {

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<!doctype html >");
            stringBuilder.AppendLine("<html>");

            stringBuilder.AppendLine("<head>");
            stringBuilder.AppendLine("  <title>Bootstrap Example</title>");
            stringBuilder.AppendLine("  <meta charset=\"utf-8\">");
            stringBuilder.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            stringBuilder.AppendLine("  <link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.2.1/css/bootstrap.min.css\">");
            stringBuilder.AppendLine("  <script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script>");
            stringBuilder.AppendLine("  <script src=\"https://stackpath.bootstrapcdn.com/bootstrap/4.2.1/js/bootstrap.min.js\"></script>");
            stringBuilder.AppendLine("</head>");

            stringBuilder.AppendLine("<table class=\"table table-dark\">");

            stringBuilder.Append($"<thead><tr><th scope=\"col\">EndpointName</th><th scope=\"col\">Message</th><th scope=\"col\">Environment</th><th scope=\"col\">Version</th></tr></thead>");

            if (Cache[CacheKey] is List<IEndpointRegistration> endpointRegistrations)
            {
                endpointRegistrations.ForEach((reg) =>
                {
                    stringBuilder.Append($"<tr><th scope=\"row\">{reg.EndpointName}</th><td>{reg.Message}</td><td>{reg.Environment}</td><td>{reg.Version}</td></tr>");
                });
            }
            stringBuilder.AppendLine("</table>");
            stringBuilder.AppendLine("</html>");

            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent(stringBuilder.ToString(), Encoding.UTF8, MediaTypeNames.Text.Html);

            return httpResponseMessage;
        }
    }
}
