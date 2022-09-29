using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Toolbelt.Blazor.Test.Internals;
using Xunit;

namespace Toolbelt.Blazor.Test
{
    public class HttpClientInterceptorTest
    {
        private void GetServiceTest(Action<IServiceCollection> configure = null)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClientInterceptor();

            configure?.Invoke(services);

            var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            serviceProvider.GetService<HttpClientInterceptor>().IsNotNull();

            using var scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetService<HttpClientInterceptor>().IsNotNull();
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

        [Theory]
        [InlineData(true, 0, HttpStatusCode.BadRequest)]
        [InlineData(false, 1, HttpStatusCode.NoContent)]
        public async Task CancelRequest_Test(
            bool cancel,
            int countOfAfterSendExpected,
            HttpStatusCode responseStatusCodeExpected)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClientInterceptor();
            services.AddSingleton<HttpClient>(sp => new HttpClient(new NullHttpMessageHandler()).EnableIntercept(sp));

            using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            var httpClientInterceptor = serviceProvider.GetService<HttpClientInterceptor>();

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest); // set to BadRequest for test purpose
            var countOfAfterSend = 0;
            httpClientInterceptor.BeforeSend += (_, args) => args.Cancel = cancel;
            httpClientInterceptor.AfterSend += (_, args) =>
            {
                countOfAfterSend++;
                response = args.Response;
            };

            var httpClient = serviceProvider.GetService<HttpClient>();
            await httpClient.GetAsync("http://example.com/foo/bar");

            countOfAfterSend.Is(countOfAfterSendExpected);
            response.StatusCode.Is(responseStatusCodeExpected);
        }

        [Fact]
        public async Task InvalidContentHeader_Test()
        {
            // Given
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClientInterceptor();
            services.AddSingleton<HttpClient>(sp => new HttpClient().EnableIntercept(sp));
            using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            var httpClientInterceptor = serviceProvider.GetService<HttpClientInterceptor>();

            var success = false;
            httpClientInterceptor.AfterSendAsync += async (_, args) =>
            {
                args.Response.StatusCode.Is(HttpStatusCode.InternalServerError);
                var conetnt = await args.GetCapturedContentAsync();
                var body = await conetnt.ReadAsStringAsync();
                body.Is("{\"status\":500,\"error\":\"hello.\"}");
                success = true;
            };

            // When
            using var testApiServer = await TestApiServer.StartAsync<TestApiServer.App1>();
            var httpClient = serviceProvider.GetService<HttpClient>();
            await httpClient.GetAsync($"http://127.0.0.1:{testApiServer.Port}/");

            // Then
            success.IsTrue();
        }
    }
}
