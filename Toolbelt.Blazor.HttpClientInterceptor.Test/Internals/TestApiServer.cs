using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.Blazor.Test.Internals
{
    public class TestApiServer : IDisposable
    {
        public class App1
        {
            public void Configure(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    context.Response.Headers.Add("Expires", "-1");
                    await context.Response.WriteAsync("{\"status\":500,\"error\":\"hello.\"}");
                });
            }
        }

        public static async Task<TestApiServer> StartAsync<T>() where T : class
        {
            var tcpPort = GetAvailableTcpPort();
            var host = new WebHostBuilder()
                .UseStartup<T>()
                .UseKestrel()
                .UseUrls($"http://127.0.0.1:{tcpPort}")
                .Build();
            await host.StartAsync();
            return new TestApiServer(host, tcpPort);
        }

        private static int GetAvailableTcpPort()
        {
            var usedTcpPorts = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Select(listener => listener.Port).ToHashSet();
            var availabeTcpPort = Enumerable.Range(5000, 1000).FirstOrDefault(port => !usedTcpPorts.Contains(port));
            if (availabeTcpPort == 0) throw new Exception($"There is no avaliable TCP port in range 5000~5999.");
            return availabeTcpPort;
        }

        private readonly IWebHost _WebHost;

        public int Port { get; }

        private TestApiServer(IWebHost webHost, int port)
        {
            this._WebHost = webHost;
            this.Port = port;
        }

        public void Dispose() => this._WebHost?.Dispose();
    }
}
