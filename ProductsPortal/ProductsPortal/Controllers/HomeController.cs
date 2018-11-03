using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using Microsoft.ServiceBus;
using ProductsPortal.Models;
using ProductsServer;

namespace ProductsPortal.Controllers
{
    public class HomeController : Controller
    {
        // Declare the channel factory.
        static ChannelFactory<IProductsChannel> channelFactory;
        public HomeController()
        {
            // Create shared access signature token credentials for authentication.
            channelFactory = new ChannelFactory<IProductsChannel>(new NetTcpRelayBinding(),
                "sb://nkrelay.servicebus.windows.net/products");
            channelFactory.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior
            {
                TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                    "RootManageSharedAccessKey", "UDM6HmMxBkd4VqbRseeglTf6lpLsGojf6+O7trcogS0=")
            });
        }

        public ActionResult Index(string Identifier, string ProductName)
        {

            using (IProductsChannel channel = channelFactory.CreateChannel())
            {
                // Return a view of the products inventory.
                return this.View(from prod in channel.GetProducts()
                    select
                        new Product
                        {
                            Id = prod.Id,
                            Name = prod.Name,
                            Quantity = prod.Quantity
                        });
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}