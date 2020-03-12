using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace SampleSite.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddHttpClientInterceptor();
            builder.Services.AddBaseAddressHttpClient();

            var host = builder.Build();
            SubscribeHttpClientInterceptorEvents(host);

            await host.RunAsync();
        }

        private static void SubscribeHttpClientInterceptorEvents(WebAssemblyHost host)
        {
            // Subscribe HttpClientInterceptor's events.
            var httpInterceptor = host.Services.GetService<HttpClientInterceptor>();
            httpInterceptor.BeforeSend += OnBeforeSend;
            httpInterceptor.AfterSend += OnAfterSend;
        }

        private static void OnBeforeSend(object sender, HttpClientInterceptorEventArgs args)
        {
            Console.WriteLine("BeforeSend event of HttpClientInterceptor");
            Console.WriteLine($"  - {args.Request.Method} {args.Request.RequestUri}");
        }

        private static void OnAfterSend(object sender, HttpClientInterceptorEventArgs args)
        {
            Console.WriteLine("AfterSend event of HttpClientInterceptor");
            Console.WriteLine($"  - {args.Request.Method} {args.Request.RequestUri}");
            Console.WriteLine($"  - HTTP Status {args.Response?.StatusCode}");
        }
    }
}
