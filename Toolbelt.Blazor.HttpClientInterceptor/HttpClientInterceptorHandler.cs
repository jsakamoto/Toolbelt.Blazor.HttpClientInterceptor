using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Toolbelt.Blazor
{
    internal class HttpClientInterceptorHandler : HttpMessageHandler
    {
        private static readonly MethodInfo SendAsyncMethod = typeof(HttpMessageHandler).GetMethod(nameof(SendAsync), BindingFlags.Instance | BindingFlags.NonPublic);

        private HttpClientInterceptor Interceptor;

        private readonly IServiceProvider Services;

        private readonly HttpMessageHandler BaseHandler;

        public HttpClientInterceptorHandler(IServiceProvider services, HttpMessageHandler baseHandler)
        {
            this.Services = services;
            this.BaseHandler = baseHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (this.Interceptor == null)
            {
                this.Interceptor = this.Services.GetService<HttpClientInterceptor>();
            }
            var response = default(HttpResponseMessage);
            var exception = default(Exception);
            var args = new HttpClientInterceptorEventArgs(request);
            try
            {
                await this.Interceptor.InvokeBeforeSendAsync(args);
                if (args.Cancel)
                {
                    response = new HttpResponseMessage(HttpStatusCode.NoContent);
                }
                else
                {
                    response = await (SendAsyncMethod.Invoke(this.BaseHandler, new object[] { request, cancellationToken }) as Task<HttpResponseMessage>);
                }
                return response;
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                if (!args.Cancel)
                {
                    var argsAfter = new HttpClientInterceptorEventArgs(request, response, exception);
                    await this.Interceptor.InvokeAfterSendAsync(argsAfter);
                    await args._AsyncTask;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.BaseHandler?.Dispose();
            base.Dispose(disposing);
        }
    }
}
