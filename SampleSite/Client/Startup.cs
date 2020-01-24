using System;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace SampleSite.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds a HttpClientInterceptor to DI.
            services.AddHttpClientInterceptor();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            // Subscribe HttpClientInterceptor's events.
            var httpInterceptor = app.Services.GetService<HttpClientInterceptor>();
            httpInterceptor.BeforeSend += OnBeforeSend;
            httpInterceptor.AfterSend += OnAfterSend;

            app.AddComponent<App>("app");
        }

        private void OnBeforeSend(object sender, HttpClientInterceptorEventArgs args)
        {
            Console.WriteLine("BeforeSend event of HttpClientInterceptor");
            Console.WriteLine($"  - {args.Request.Method} {args.Request.RequestUri}");

        }

        private void OnAfterSend(object sender, HttpClientInterceptorEventArgs args)
        {
            Console.WriteLine("AfterSend event of HttpClientInterceptor");
            Console.WriteLine($"  - {args.Request.Method} {args.Request.RequestUri}");
            Console.WriteLine($"  - HTTP Status {args.Response?.StatusCode}");
        }
    }
}
