using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbelt.Blazor.Test.Internals
{
    internal class NullHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            return Task.FromResult(responseMessage);
        }
    }
}