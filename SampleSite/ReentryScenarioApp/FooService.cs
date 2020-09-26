using System;
using System.Net.Http;
using System.Threading.Tasks;
using Toolbelt.Blazor;

namespace ReentryScenarioApp
{
    public class FooService : IDisposable
    {
        private readonly HttpClient HttpClient;

        private readonly HttpClientInterceptor HttpClientInterceptor;

        public FooService(
            IHttpClientFactory httpClientFactory,
            HttpClientInterceptor httpClientInterceptor
        )
        {
            this.HttpClient = httpClientFactory.CreateClient("for FooService");
            this.HttpClientInterceptor = httpClientInterceptor;

            this.HttpClientInterceptor.BeforeSend += HttpClientInterceptor_BeforeSend;
            this.HttpClientInterceptor.BeforeSendAsync += HttpClientInterceptor_BeforeSendAsync;
            this.HttpClientInterceptor.AfterSend += HttpClientInterceptor_AfterSend;
            this.HttpClientInterceptor.AfterSendAsync += HttpClientInterceptor_AfterSendAsync;
        }

        private void HttpClientInterceptor_BeforeSend(object sender, HttpClientInterceptorEventArgs e)
        {
            Console.WriteLine("HttpClientInterceptor.BeforeSend");
        }

        private async Task HttpClientInterceptor_BeforeSendAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            Console.WriteLine("HttpClientInterceptor.BeforeSendAsync-1");

            var sampleTextB = await this.HttpClient.GetStringAsync("content/sample-text-B.txt");
            Console.WriteLine($"Sample Text B: {sampleTextB}");

            Console.WriteLine("HttpClientInterceptor.BeforeSendAsync-2");
        }

        private void HttpClientInterceptor_AfterSend(object sender, HttpClientInterceptorEventArgs e)
        {
            Console.WriteLine("HttpClientInterceptor.AfterSend");
        }

        private async Task HttpClientInterceptor_AfterSendAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            Console.WriteLine("HttpClientInterceptor.AfterSendAsync-1");
            await Task.Delay(3000);
            Console.WriteLine("HttpClientInterceptor.AfterSendAsync-2");
        }

        public void Dispose()
        {
            this.HttpClientInterceptor.BeforeSend -= HttpClientInterceptor_BeforeSend;
            this.HttpClientInterceptor.BeforeSendAsync -= HttpClientInterceptor_BeforeSendAsync;
            this.HttpClientInterceptor.AfterSend -= HttpClientInterceptor_AfterSend;
            this.HttpClientInterceptor.AfterSendAsync -= HttpClientInterceptor_AfterSendAsync;
            this.HttpClient.Dispose();
        }
    }
}
