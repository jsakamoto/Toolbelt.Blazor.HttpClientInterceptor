using System.Threading.Tasks;

namespace Toolbelt.Blazor
{
    public delegate Task HttpClientInterceptorEventHandler(object sender, HttpClientInterceptorEventArgs e);
}
