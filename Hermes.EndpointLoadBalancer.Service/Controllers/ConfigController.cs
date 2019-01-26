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
using Newtonsoft.Json;
using NServiceBus.Logging;

namespace Hermes.EndpointLoadBalancer.Service.Controllers
{
    [RoutePrefix("api/Config")]
    public class ConfigController : ApiController
    {

        private static readonly ObjectCache Cache = MemoryCache.Default;
        private const string CacheKey = "endpointRegistrations";

        static readonly ILog log = LogManager.GetLogger<Host>();

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

            log.Debug("Begin RegisterEndpoint");

            HttpResponseMessage httpResponseMessage;

            if (Cache[CacheKey] is List<IEndpointRegistration> endpointRegistrations)
            {
                if (!endpointRegistrations.Contains(endpointRegistration))
                {
                    log.Debug($"Registering endpoint: {endpointRegistration.ToString()}");
                    endpointRegistrations.Add(endpointRegistration);
                }
                else
                {
                    log.Debug("Endpoint is already registered.  Calling UpdateEndpointRegistration.");
                    UpdateEndpointRegistration(endpointRegistration);
                }

                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
                httpResponseMessage.Content = new StringContent("Unable to load endpointRegistration list from memory.", Encoding.UTF8, MediaTypeNames.Text.Plain);
            }

            log.Debug("End RegisterEndpoint");

            return httpResponseMessage;
        }

        [Route("UpdateEndpointRegistration")]
        [HttpPut]
        public HttpResponseMessage UpdateEndpointRegistration(EndpointRegistration endpointRegistration)
        {

            log.Debug("Begin UpdateEndpointRegistration");

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

            log.Debug("End UpdateEndpointRegistration");

            return httpResponseMessage;
        }

        [Route("UnRegisterEndpoint")]
        [HttpDelete]
        public HttpResponseMessage UnRegisterEndpoint(EndpointRegistration endpointRegistration)
        {

            log.Debug("Begin UnRegisterEndpoint");

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

            log.Debug("End UnRegisterEndpoint");

            return httpResponseMessage;
        }

        [Route("ShowRegisteredEndpoints")]
        [HttpGet]
        public HttpResponseMessage ShowRegisteredEndpoints()
        {

            log.Debug("Begin ShowRegisteredEndpoints");

            string registeredEndpoints = null;

            if (Cache[CacheKey] is List<IEndpointRegistration> endpointRegistrations)
            {
                registeredEndpoints = JsonConvert.SerializeObject(Cache[CacheKey]);
            }

            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent(registeredEndpoints, Encoding.UTF8, "application/json");

            log.Debug("End ShowRegisteredEndpoints");

            return httpResponseMessage;
        }
    }
}
