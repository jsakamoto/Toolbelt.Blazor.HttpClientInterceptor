using System;
using System.Net.Http;

namespace Toolbelt.Blazor
{
    public class HttpClientInterceptorEventArgs : EventArgs
    {
        public HttpRequestMessage Request { get; }

        public HttpResponseMessage Response { get; }

        public HttpClientInterceptorEventArgs(HttpRequestMessage request, HttpResponseMessage response)
        {
            this.Request = request;
            this.Response = response;
        }
    }
}
