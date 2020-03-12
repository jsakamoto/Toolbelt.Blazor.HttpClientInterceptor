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
        /// <summary>
        ///  Adds a HttpClientInterceptor service to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        public static void AddHttpClientInterceptor(this IServiceCollection services)
        {
            services.TryAddSingleton(serviceProvider =>
            {
                var httpClient = serviceProvider.GetRequiredService<HttpClient>();
                var handlerField = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic);
                var baseHandler = handlerField?.GetValue(httpClient) as HttpMessageHandler;

                var interceptor = new HttpClientInterceptor(baseHandler);
                if (handlerField != null && baseHandler != null)
                    handlerField.SetValue(httpClient, interceptor);

                return interceptor;
            });
        }
    }
}
