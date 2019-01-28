using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DocumentGenerationMessage;
using DocumentGenerationMessage.Adobe;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Features;

namespace Hermes.Test.Web.Controllers
{
    public class DataController : ApiController
    {
        [HttpGet]
        public string GetTime()
        {
            return DateTime.Now.ToString("O");
        }

        [HttpGet]
        public async Task<HttpResponseMessage> SendMessage()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration("Hermes.Test.Web");

                endpointConfiguration.UseTransport<MsmqTransport>();
                endpointConfiguration.DisableFeature<MessageDrivenSubscriptions>();
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

                endpointConfiguration.SendOnly();

                var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

                IDocumentGenerationMessage documentGenerationMessageAdobe = new DocumentGenerationMessageAdobe()
                {
                    Environment = "DEV",
                    From = endpointConfiguration.GetSettings().EndpointName(),
                    To = "Hermes.EndpointLoadBalancer.Service",
                    Version = "1.0.0",
                    Message = typeof(IDocumentGenerationMessage).Name,
                    DocumentName = "Sample Document"
                };

                await endpointInstance.Send("Hermes.EndpointLoadBalancer.Service", documentGenerationMessageAdobe);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Message sent.", Encoding.UTF8)
            };
        }
    }
}
