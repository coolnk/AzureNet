using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;

namespace HybridServer
{
    class Program
    {
        private const string RelayNamespace = "nkrelay.servicebus.windows.net";
        private const string ConnectionName = "nv1";
        private const string KeyName = "ListenKey";
        private const string Key = "Koxt4j7iBS8GxRirKuk0qybk4dJyrjXoVyR634YMESs=";
        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }
        private static async Task RunAsync()
        {
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
            var listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", RelayNamespace, ConnectionName)), tokenProvider);

            // Subscribe to the status events.
            listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
            listener.Offline += (o, e) => { Console.WriteLine("Offline"); };
            listener.Online += (o, e) => { Console.WriteLine("Online"); };

            // Provide an HTTP request handler
            listener.RequestHandler = (context) =>
            {
                if (true)
                {
                    // Do something with context.Request.Url, HttpMethod, Headers, InputStream...
                    context.Response.StatusCode = HttpStatusCode.OK;
                    context.Response.StatusDescription = "OK";
                    var body = new StreamReader(context.Request.InputStream).ReadToEnd();
                    using (var sw = new StreamWriter(context.Response.OutputStream))
                    {
                        

                        sw.WriteLine("hello");
                    }


                }

                if (true)
                {
                    // Do something with context.Request.Url, HttpMethod, Headers, InputStream...
                    context.Response.StatusCode = HttpStatusCode.OK;
                    context.Response.StatusDescription = "OK";
                    using (var sw = new StreamWriter(context.Response.OutputStream))
                    {
                        sw.WriteLine("world!");
                    }


                }



                // The context MUST be closed here
                context.Response.Close();
            };

            // Opening the listener establishes the control channel to
            // the Azure Relay service. The control channel is continuously 
            // maintained, and is reestablished when connectivity is disrupted.
            await listener.OpenAsync();
            Console.WriteLine("Server listening");

            // Start a new thread that will continuously read the console.
            await Console.In.ReadLineAsync();

            // Close the listener after you exit the processing loop.
            await listener.CloseAsync();
        }
    }
}
