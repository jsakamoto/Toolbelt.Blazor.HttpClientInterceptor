using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Toolbelt.Blazor.Test.Internals;
using Xunit;

namespace Toolbelt.Blazor.Test
{
    public class IHttpClientInterceptorTest
    {
        private void GetServiceTest(Action<IServiceCollection> configure = null)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClientInterceptor();

            configure?.Invoke(services);

            var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            serviceProvider.GetService<IHttpClientInterceptor>().IsNotNull();

            using var scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetService<IHttpClientInterceptor>().IsNotNull();
        }

        [Fact]
        public void HttpClient_as_a_Singleton_Test()
        {
            this.GetServiceTest(configure: services =>
            {
                services.AddSingleton<HttpClient>(sp => new HttpClient());
            });
        }

        [Fact]
        public void HttpClient_as_a_Transient_Test()
        {
            this.GetServiceTest(configure: services =>
            {
                services.AddTransient<HttpClient>(sp => new HttpClient());
            });
        }

        [Fact]
        public void HttpClient_as_a_Scoped_Test()
        {
            this.GetServiceTest(configure: services =>
            {
                services.AddScoped<HttpClient>(sp => new HttpClient());
            });
        }

        [Fact]
        public void No_HttpClient_Registration_Test()
        {
            this.GetServiceTest(/* doesn't register HttpClient to DI container. */);
        }

        [Fact]
        public async Task EventCount_with_SingleTonHttpClient_Test()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClientInterceptor();
            services.AddSingleton<HttpClient>(sp => new HttpClient(new NullHttpMessageHandler()).EnableIntercept(sp));

            using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            var httpClientInterceptor = serviceProvider.GetService<HttpClientInterceptor>();

            var countOfBeforeSend = 0;
            var countOfAfterSend = 0;
            httpClientInterceptor.BeforeSend += (_, __) => countOfBeforeSend++;
            httpClientInterceptor.AfterSend += (_, __) => countOfAfterSend++;

            var httpClient = serviceProvider.GetService<HttpClient>();
            await httpClient.GetAsync("http://example.com/foo/bar");

            countOfBeforeSend.Is(1);
            countOfAfterSend.Is(1);
        }
    }
}
