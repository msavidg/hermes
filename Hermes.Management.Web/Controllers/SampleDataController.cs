using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hermes.Common.Datatypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Hermes.Management.Web.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {

        [HttpGet("[action]")]
        public IEnumerable<EndpointRegistration> Endpoints()
        {
            List<EndpointRegistration> endpoints = new List<EndpointRegistration>();

            using (WebClient webClient = new WebClient())
            {
                string result = webClient.DownloadString("http://localhost:9000/api/Config/ShowRegisteredEndpoints");
                endpoints = JsonConvert.DeserializeObject<List<EndpointRegistration>>(result);
            }

            return endpoints;
        }
    }
}
