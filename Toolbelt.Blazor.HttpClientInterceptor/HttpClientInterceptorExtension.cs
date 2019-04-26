using System.Linq;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

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
            if (services.FirstOrDefault(d => d.ServiceType == typeof(HttpClientInterceptor)) == null)
            {
                services.AddSingleton(_ => new HttpClientInterceptor());
            }
        }

        /// <summary>
        ///  Installs a HttpClientInterceptor service to the runtime hosting environment.
        /// </summary>
        /// <param name="app">The Microsoft.AspNetCore.Blazor.Builder.IBlazorApplicationBuilder.</param>
        public static IComponentsApplicationBuilder UseHttpClientInterceptor(this IComponentsApplicationBuilder app)
        {
            var interceptor = app.Services.GetService<HttpClientInterceptor>();
            interceptor.Install();

            return app;
        }
    }
}
