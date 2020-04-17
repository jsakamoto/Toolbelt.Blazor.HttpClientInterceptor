# Blazor WebAssembly (client-side) HttpClient Interceptor [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.Blazor.HttpClientInterceptor.svg)](https://www.nuget.org/packages/Toolbelt.Blazor.HttpClientInterceptor/)

## Summary

The class library that intercept all of the sending HTTP requests on a client side Blazor WebAssembly application.

## Supported Blazor versions

"Blazor WebAssembly App (client-side) HttpClient Interceptor" ver.8.x supports Blazor WebAssembly App version **3.2 Preview 2~4.**

## How to install and use?

**Step.1** Install the library via NuGet package, like this.

```shell
> dotnet add package Toolbelt.Blazor.HttpClientInterceptor
```

**Step.2** Register "HttpClientInterceptor" service into the DI container, at `Main()` method in the `Program` class of your Blazor application.

```csharp
using Toolbelt.Blazor.Extensions.DependencyInjection; // <- Add this, and...
...
public class Program
{
  public static async Task Main(string[] args)
  {
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("app");
    builder.Services.AddHttpClientInterceptor(); // <- Add this!
    ...
```

That's all.

You can subscribe the events that will occur before/after sending all of the HTTP requests, at anywhere you can get HttpClientInterceptor service from the DI container.

```csharp
@using Toolbelt.Blazor
@inject HttpClientInterceptor Interceptor
...
@code {
  protected override void OnInitialized()
  {
    this.Interceptor.BeforeSend += Interceptor_BeforeSend;
    ...
  }

  void Interceptor_BeforeSend(object sender, HttpClientInterceptorEventArgs e)
  {
    // Do something here what you want to do.
  }
  ...
```

> _Note:_ Please remember that if you use `HttpClientInterceptor` to subscribe `BeforeSend`/`AfterSend` events **in Blazor components (.razor),** you should unsubscribe events when the components is discarded. To do it, you should implement `IDisposable` interface in that component, like this code:

```csharp
@implements IDisposable
...
public void Dispose()
{
  this.Interceptor.BeforeSend -= Interceptor_BeforeSend;
}
```

### The arguments of event handler

The event handler for `BeforeSend`/`AfterSend` events can be received `HttpClientInterceptorEventArgs` object.

The `HttpClientInterceptorEventArgs` object provides you to a request object and a response object that is come from an intercepted HttpClinet request.

```csharp
void OnAfterSend(object sender, HttpClientInterceptorEventArgs args)
{
  if (!args.Response?.IsSuccessStatusCode) {
    // Do somthing here for handle all errors of HttpClient requests.
  }
}
```

## Release Note

- **v.8.0.1** - Fix: conflict "AddBaseAddressHttpClient()" service injection.
- **v.8.0.0** - BREAKING CHANGE: Support Blazor v.3.2.0 Preview 2 (not compatible with v.3.2.0 Preview 1 or before.)
- **v.7.0.0** - BREAKING CHANGE: Support Blazor v.3.2.0 Preview 1 (not compatible with v.3.1.0 Preview 4 or before.)
- **v.6.1.0** - The event handler arguments now provides a request object and a response object.
- **v.6.0.0** - BREAKING CHANGE: Support Blazor v.3.1.0 Preview 3 (not compatible with v.3.1.0 Preview 2 or before.)
- **v.5.0.0** - BREAKING CHANGE: Support Blazor v.3.0.0 Preview 6 (not compatible with v.3.0.0 Preview 5 or before.)
- **v.4.0.0** - BREAKING CHANGE: Support Blazor v.3.0.0 Preview 4 (not compatible with v.0.9.0 or before.)
- **v.3.0.0** - BREAKING CHANGE: Support Blazor v.0.8.0 (not compatible with v.0.7.0 or before.)
- **v.2.1.0** - Support Blazor v.0.6.0 - it was signed strong name.
- **v.2.0.0** - BREAKING CHANGE: Fix namespace of HttpClientInterceptorExtension class.
- **v.1.0.0** - 1st release.

## License

[Mozilla Public License Version 2.0](https://github.com/jsakamoto/Toolbelt.Blazor.HttpClientInterceptor/blob/master/LICENSE)
