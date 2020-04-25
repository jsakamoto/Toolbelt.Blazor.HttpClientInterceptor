using System;

namespace Toolbelt.Blazor
{
    /// <summary>
    /// Intercept all of the sending HTTP requests on a client side Blazor application.
    /// </summary>
    public class HttpClientInterceptor
    {
        /// <summary>
        /// Occurs before a HTTP request sending.
        /// </summary>
        public event EventHandler<HttpClientInterceptorEventArgs> BeforeSend;

        /// <summary>
        /// Occurs after received a response of a HTTP request. (include it wasn't succeeded.)
        /// </summary>
        public event EventHandler<HttpClientInterceptorEventArgs> AfterSend;

        internal HttpClientInterceptor()
        {
        }

        internal void InvokeBeforeSend(HttpClientInterceptorEventArgs args)
        {
            this.BeforeSend?.Invoke(this, args);
        }

        internal void InvokeAfterSend(HttpClientInterceptorEventArgs args)
        {
            this.AfterSend?.Invoke(this, args);
        }
    }
}
