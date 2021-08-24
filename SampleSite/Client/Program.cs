using System;
using System.Linq;
using System.Net.Http;
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
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }.EnableIntercept(sp));

            var host = builder.Build();
            SubscribeHttpClientInterceptorEvents(host);

            await host.RunAsync();
        }

        private static void SubscribeHttpClientInterceptorEvents(WebAssemblyHost host)
        {
            // Subscribe IHttpClientInterceptor's events.
            var httpInterceptor = host.Services.GetService<IHttpClientInterceptor>();
            httpInterceptor.BeforeSend += OnBeforeSend;
            httpInterceptor.AfterSendAsync += OnAfterSendAsync;
        }

        private static void OnBeforeSend(object sender, HttpClientInterceptorEventArgs args)
        {
            Console.WriteLine("BeforeSend event of HttpClientInterceptor");
            Console.WriteLine($"  - {args.Request.Method} {args.Request.RequestUri}");
        }

        private static async Task OnAfterSendAsync(object sender, HttpClientInterceptorEventArgs args)
        {
            Console.WriteLine("AfterSend event of HttpClientInterceptor");
            Console.WriteLine($"  - {args.Request.Method} {args.Request.RequestUri}");
            Console.WriteLine($"  - HTTP Status {args.Response?.StatusCode}");

            var capturedContent = await args.GetCapturedContentAsync();

            Console.WriteLine($"  - Content Headers");
            foreach (var headerText in capturedContent.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))
            {
                Console.WriteLine($"    - {headerText}");
            }

            var httpContentString = await capturedContent.ReadAsStringAsync();
            Console.WriteLine($"  - HTTP Content \"{httpContentString}\"");
        }
    }
}
