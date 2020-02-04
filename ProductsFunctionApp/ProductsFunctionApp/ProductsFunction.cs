using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus;
using Newtonsoft.Json;
using ProductsServer;

namespace ProductsFunctionApp
{

    public static class ProductsFunction
    {
        static ChannelFactory<IProductsChannel> channelFactory;
        [FunctionName("products")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            var remoteAddress =  GetEnvironmentVariable("RemoteAddress");
            var rootManageSharedAccessKey = GetEnvironmentVariable("RootManageSharedAccessKey");
            var sendKeyName = GetEnvironmentVariable("SendKeyName");


            // Create shared access signature token credentials for authentication.
            channelFactory = new ChannelFactory<IProductsChannel>(new NetTcpRelayBinding(),
                remoteAddress);
            channelFactory.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior
            {
                TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                    sendKeyName, rootManageSharedAccessKey)
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

                string json = JsonConvert.SerializeObject(result, Formatting.Indented);
                log.Info(json);

                return req.CreateResponse(HttpStatusCode.OK, result);
            }
        }

        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
