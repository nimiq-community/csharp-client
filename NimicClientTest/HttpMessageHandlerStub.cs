using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Nimiq;

namespace NimiqClientTest
{
    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        // test data
        public static string TestData { get; set; }
        public static Dictionary<string, object> LatestRequest { get; set; }
        public static string LatestRequestMethod { get; set; }
        public static object[] LatestRequestParams { get; set; }

        // send back the resonse
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LatestRequest = null;
            LatestRequestMethod = null;
            LatestRequestParams = null;

            var content = request.Content;
            var json = content.ReadAsStringAsync().Result;

            LatestRequest = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            LatestRequestMethod = (string)((JsonElement)LatestRequest["method"]).GetObject();
            LatestRequestParams = (object[])((JsonElement)LatestRequest["params"]).GetObject();

            // load test data
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(TestData)
            };

            return await Task.FromResult(responseMessage);
        }
    }
}
