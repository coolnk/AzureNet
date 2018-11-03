using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus;
using ProductsServer;

namespace ProductsFunctionApp
{
  
    public static class ProductsFunction
    {
        static ChannelFactory<IProductsChannel> channelFactory;
        [FunctionName("products")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
         
            // Create shared access signature token credentials for authentication.
            channelFactory = new ChannelFactory<IProductsChannel>(new NetTcpRelayBinding(),
                "sb://nkrelay.servicebus.windows.net/products");
            channelFactory.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior
            {
                TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                    "RootManageSharedAccessKey", "UDM6HmMxBkd4VqbRseeglTf6lpLsGojf6+O7trcogS0=")
            });

            log.Info("C# HTTP trigger function processed a request.");

            using (IProductsChannel channel = channelFactory.CreateChannel())
            {
                // Return a view of the products inventory.
               var result  = from prod in channel.GetProducts()
                    select
                        new Product
                        {
                            Id = prod.Id,
                            Name = prod.Name,
                            Quantity = prod.Quantity
                        };
               return req.CreateResponse(HttpStatusCode.OK, result);
            }
        }
    }
}
