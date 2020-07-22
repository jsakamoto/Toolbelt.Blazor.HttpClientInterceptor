using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Xunit;

namespace Toolbelt.Blazor.Test
{
    public class HttpClientInterceptorTest
    {
        private void GetServiceTest(Action<IServiceCollection> configure = null)
        {
            var services = new ServiceCollection();
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
            GetServiceTest(configure: services =>
            {
                services.AddSingleton<HttpClient>(sp => new HttpClient());
            });
        }

        [Fact]
        public void HttpClient_as_a_Transient_Test()
        {
            GetServiceTest(configure: services =>
            {
                services.AddTransient<HttpClient>(sp => new HttpClient());
            });
        }

        [Fact]
        public void HttpClient_as_a_Scoped_Test()
        {
            GetServiceTest(configure: services =>
            {
                services.AddScoped<HttpClient>(sp => new HttpClient());
            });
        }

        [Fact]
        public void No_HttpClient_Registration_Test()
        {
            GetServiceTest(/* doesn't register HttpClient to DI container. */);
        }
    }
}
