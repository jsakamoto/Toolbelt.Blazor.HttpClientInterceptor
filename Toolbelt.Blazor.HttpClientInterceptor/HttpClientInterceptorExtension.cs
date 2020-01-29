using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
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
            var baseHandlerType = Type.GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler,WebAssembly.Net.Http");
            if (baseHandlerType == null) return;
            var baseHandler = Activator.CreateInstance(baseHandlerType) as HttpMessageHandler;
            if (baseHandler == null) return;

            services.TryAddSingleton(_ => new HttpClientInterceptor(baseHandler));

            services.RemoveAll<HttpClient>();
            services.AddSingleton(delegate (IServiceProvider s)
            {
                var naviManager = s.GetRequiredService<NavigationManager>();
                var interceptor = s.GetRequiredService<HttpClientInterceptor>();
                return new HttpClient(interceptor)
                {
                    BaseAddress = new Uri(naviManager.BaseUri)
                };
            });
        }
    }
}
