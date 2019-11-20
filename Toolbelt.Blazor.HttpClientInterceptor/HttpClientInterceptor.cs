using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbelt.Blazor
{
    /// <summary>
    /// Intercept all of the sending HTTP requests on a client side Blazor application.
    /// </summary>
    public class HttpClientInterceptor : HttpMessageHandler
    {
        /// <summary>
        /// Occurs before a HTTP request sending.
        /// </summary>
        public event EventHandler BeforeSend;

        /// <summary>
        /// Occurs after received a response of a HTTP request. (include it wasn't succeeded.)
        /// </summary>
        public event EventHandler AfterSend;

        private HttpMessageHandler Handler;

        private readonly MethodInfo SendAsyncMethod;

        internal HttpClientInterceptor(HttpMessageHandler baseHandler)
        {
            this.SendAsyncMethod = typeof(HttpMessageHandler).GetMethod(nameof(SendAsync), BindingFlags.Instance | BindingFlags.NonPublic);
            this.Handler = baseHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                this.BeforeSend?.Invoke(this, EventArgs.Empty);
                return await (this.SendAsyncMethod.Invoke(this.Handler, new object[] { request, cancellationToken }) as Task<HttpResponseMessage>);
            }
            finally
            {
                this.AfterSend?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void Dispose(bool disposing)
        {
            Handler?.Dispose();
            base.Dispose(disposing);
        }
    }
}
