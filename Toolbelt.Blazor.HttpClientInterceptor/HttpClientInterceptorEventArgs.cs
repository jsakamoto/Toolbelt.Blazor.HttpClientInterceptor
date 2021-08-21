using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
        /// Cancel sending HTTP request
        /// </summary>
        public bool Cancel { get; set; }

        private byte[] _CapturedContentBytes;

        private HttpContentHeaders _CapturedContentHeaders;

        internal Task _AsyncTask = Task.CompletedTask;

        /// <summary>
        /// Provides data for the event that is raised when before or after sending HTTP request.
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        public HttpClientInterceptorEventArgs(HttpRequestMessage request, HttpResponseMessage response)
        {
            this.Request = request;
            this.Response = response;
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

            if (_CapturedContentBytes == null)
            {
                _AsyncTask = CaptureContentAsync();
                await _AsyncTask;
            }

            var httpContent = new ReadOnlyMemoryContent(_CapturedContentBytes);
            foreach (var contentHeader in _CapturedContentHeaders)
            {
                httpContent.Headers.Add(contentHeader.Key, contentHeader.Value);
            }
            return httpContent;
        }

        private async Task CaptureContentAsync()
        {
            await Response.Content.LoadIntoBufferAsync();

            _CapturedContentHeaders = Response.Content.Headers;

            _CapturedContentBytes = await Response.Content.ReadAsByteArrayAsync();
            var stream = await Response.Content.ReadAsStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
