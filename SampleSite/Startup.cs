using System;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace SampleSite
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds a HttpClientInterceptor to DI.
            services.AddHttpClientInterceptor();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            // Install a HttpClientInterceptor.
            app.UseHttpClientInterceptor();

            // Subscribe HttpClientInterceptor's events.
            var httpInterceptor = app.Services.GetService<HttpClientInterceptor>();
            httpInterceptor.BeforeSend += OnBeforeSend;
            httpInterceptor.AfterSend += OnAfterSend;

            app.AddComponent<App>("app");
        }

        private void OnBeforeSend(object sender, EventArgs args)
        {
            Console.WriteLine("BeforeSend event of HttpClientInterceptor");
        }

        private void OnAfterSend(object sender, EventArgs args)
        {
            Console.WriteLine("AfterSend event of HttpClientInterceptor");
        }
    }
}
