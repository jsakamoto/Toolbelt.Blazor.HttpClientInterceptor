using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Toolbelt.Blazor.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding and using HttpClientInterceptor.
    /// </summary>
    public static class HttpClientInterceptorExtension
    {
        private static readonly FieldInfo HandlerField = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        ///  Adds a HttpClientInterceptor service to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        public static void AddHttpClientInterceptor(this IServiceCollection services)
        {
            services.TryAddSingleton(services =>
            {
                if (HandlerField != null)
                {
                    var httpClient = default(HttpClient);
                    try
                    {
                        httpClient = services.GetService<HttpClient>();
                    }
                    catch (InvalidOperationException e) when (e.Source == "Microsoft.Extensions.DependencyInjection" && e.HResult == -2146233079)
                    {
                    }

                    if (httpClient != null)
                    {
                        httpClient.EnableIntercept(services);
                    }
                }

                return new HttpClientInterceptor();
            });
        }

        public static HttpClient EnableIntercept(this HttpClient httpClient, IServiceProvider services)
        {
            if (HandlerField != null)
            {
                var baseHandler = HandlerField?.GetValue(httpClient) as HttpMessageHandler;
                if (baseHandler != null)
                {
                    var interceptorHandler = new HttpClientInterceptorHandler(services, baseHandler);
                    HandlerField.SetValue(httpClient, interceptorHandler);
                }
            }
            return httpClient;
        }
    }
}
