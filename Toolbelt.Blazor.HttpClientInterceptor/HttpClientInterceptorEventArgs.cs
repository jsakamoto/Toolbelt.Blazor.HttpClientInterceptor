using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Toolbelt.Blazor
{
    /// <summary>
    /// Provides data for the event that is raised when before or after sending HTTP request.
    /// </summary>
    public class HttpClientInterceptorEventArgs : EventArgs
    {
        /// <summary>
        /// The HttpRequestMessage object uses or used sending HTTP request.
        /// </summary>
        public HttpRequestMessage Request { get; }

        /// <summary>
        /// The HttpResponseMessage object that is returned from HTTP request handler.<br/>
        /// This property is available only when "AfterSend" event is fired.
        /// <para>[NOTICE]<br/>Don't retrive content from the "Response.Content" property directly.<br/>
        /// Instead, call the "GetCapturedContentAsync()" method of this event arguments object to do it.</para>
        /// </summary>
        public HttpResponseMessage Response { get; }

        /// <summary>
        /// The exception object if an HTTP request has thrown an exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Cancel sending HTTP request
        /// </summary>
        public bool Cancel { get; set; }

        private byte[] _CapturedContentBytes;

        private HttpContentHeaders _CapturedContentHeaders;

        internal Task _AsyncTask = Task.CompletedTask;

        internal ILogger _Logger;

        /// <summary>
        /// Provides data for the event that is raised when before or after sending HTTP request.
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public HttpClientInterceptorEventArgs(HttpRequestMessage request, HttpResponseMessage response) : this(request, response, exception: null)
        {
        }

        /// <summary>
        /// Provides data for the event that is raised when before sending HTTP request.
        /// </summary>
        /// <param name="request">Request</param>
        public HttpClientInterceptorEventArgs(HttpRequestMessage request) : this(request, response: null, exception: null)
        {
        }

        /// <summary>
        /// Provides data for the event that is raised when after sending HTTP request.
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        /// <param name="exception">The exception object if an HTTP request has thrown an exception.</param>
        public HttpClientInterceptorEventArgs(HttpRequestMessage request, HttpResponseMessage response, Exception exception)
        {
            this.Request = request;
            this.Response = response;
            this.Exception = exception;
            this.Cancel = false;
        }

        /// <summary>
        /// Get the HttpContent object that is returned from HTTP request.<br/>
        /// This method is available only when "AfterSend" event is fired.<br/>
        /// You can call this method multiple times.
        /// </summary>
        public async ValueTask<HttpContent> GetCapturedContentAsync()
        {
            if (this.Response == null) throw new InvalidOperationException("You can call GetCapturedContentAsync() only when \"AfterSend\" event is fired.");

            if (this._CapturedContentBytes == null)
            {
                this._AsyncTask = this.CaptureContentAsync();
                await this._AsyncTask;
            }

            var httpContent = new ReadOnlyMemoryContent(this._CapturedContentBytes);
            foreach (var contentHeader in this._CapturedContentHeaders)
            {
                try { httpContent.Headers.Add(contentHeader.Key, contentHeader.Value); }
                catch (Exception ex) { this._Logger.LogWarning(ex, ex.Message); }
            }
            return httpContent;
        }

        private async Task CaptureContentAsync()
        {
            await this.Response.Content.LoadIntoBufferAsync();

            this._CapturedContentHeaders = this.Response.Content.Headers;

            this._CapturedContentBytes = await this.Response.Content.ReadAsByteArrayAsync();
            var stream = await this.Response.Content.ReadAsStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
