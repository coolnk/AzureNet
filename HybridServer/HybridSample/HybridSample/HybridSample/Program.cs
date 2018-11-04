using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;

namespace HybridSample
{
    class Program
    {
        private const string RelayNamespace = "nkrelay.servicebus.windows.net";
        private const string ConnectionName = "nv1";
        private const string KeyName = "SendKey";
        private const string Key = "LHQ1lXBB0W4F8xtFW/PjIQIDTn7p4+cjHdK52/swxBs=";

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                KeyName, Key);
            var uri = new Uri(string.Format("https://{0}/{1}", RelayNamespace, ConnectionName));
            var token = (await tokenProvider.GetTokenAsync(uri.AbsoluteUri, TimeSpan.FromHours(1))).TokenString;
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };
            request.Headers.Add("ServiceBusAuthorization", token);
            var response = await client.SendAsync(request);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Console.ReadKey();

        }
    }
}
