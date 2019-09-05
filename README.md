# Blazor WebAssembly (client-side) HttpClient Interceptor [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.Blazor.HttpClientInterceptor.svg)](https://www.nuget.org/packages/Toolbelt.Blazor.HttpClientInterceptor/)

## Summary

The class library that intercept all of the sending HTTP requests on a client side Blazor WebAssembly application.

## Supported Blazor versions

"Blazor WebAssembly App (client-side) HttpClient Interceptor" ver.5.x supports Blazor WebAssembly App versions **from ver.3.0.0-prevew 6 to preview 9.**

## How to install and use?

**Step.1** Install the library via NuGet package, like this.

```shell
> dotnet add package Toolbelt.Blazor.HttpClientInterceptor
```

**Step.2** Register "HttpClientInterceptor" service into the DI container, at `ConfigureService` method in the `Startup` class of your Blazor application.

```csharp
using Toolbelt.Blazor.Extensions.DependencyInjection; // <- Add this, and...
...
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddHttpClientInterceptor(); // <- Add this line.
    ...
```

**Step.3** Install "HttpClientInterceptor" service to intercepting works well, at `Configure` method in the `Startup` class of your Blazor application.

```csharp
public void Configure(IComponentsApplicationBuilder app)
{
  app.UseHttpClientInterceptor(); // <- Add this line.
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

  void Interceptor_BeforeSend(object sender, EventArgs e)
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

## Release Note

- **v.5.0.0** - BREAKING CHANGE: Support Blazor v.3.0.0 Preview 6 (not compatible with v.3.0.0 Preview 5 or before.)
- **v.4.0.0** - BREAKING CHANGE: Support Blazor v.3.0.0 Preview 4 (not compatible with v.0.9.0 or before.)
- **v.3.0.0** - BREAKING CHANGE: Support Blazor v.0.8.0 (not compatible with v.0.7.0 or before.)
- **v.2.1.0** - Support Blazor v.0.6.0 - it was signed strong name.
- **v.2.0.0** - BREAKING CHANGE: Fix namespace of HttpClientInterceptorExtension class.
- **v.1.0.0** - 1st release.

## License

[Mozilla Public License Version 2.0](https://github.com/jsakamoto/Toolbelt.Blazor.HttpClientInterceptor/blob/master/LICENSE)
