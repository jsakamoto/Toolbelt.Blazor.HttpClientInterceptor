# Blazor HttpClient Interceptor [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.Blazor.HttpClientInterceptor.svg)](https://www.nuget.org/packages/Toolbelt.Blazor.HttpClientInterceptor/)

## Summary

The class library that intercept all of the sending HTTP requests on a client side Blazor application.

## How to install and use?

**Step.1** Install the library via NuGet package, like this.

```shell
> dotnet install Toolbelt.Blazor.HttpClientInterceptor
```

**Step.2** Register "HttpClientInterceptor" service into the DI container, at `ConfigureService` method in the `Startup` class of your Blazor application.

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddHttpClientInterceptor(); // <- Add this line.
  ...
```

**Step.3** Install "HttpClientInterceptor" service to intercepting works well, at `Configure` method in the `Startup` class of your Blazor application.

```csharp
public void Configure(IBlazorApplicationBuilder app)
{
  app.UseHttpClientInterceptor(); // <- Add this line.
  ...
```

That's all.

You can subscribe the events that will occur before/after sending all of the HTTP requests, at anywhere you can get HttpClientInterceptor service from the DI container.

```csharp
interceptor.BeforeSend += Interceptor_BeforeSend;
...
private void Interceptor_BeforeSend(object sender, EventArgs e)
{
  // Do something here what you want to do.
}
```

## License

[Mozilla Public License Version 2.0](https://github.com/jsakamoto/Toolbelt.Blazor.HttpClientInterceptor/blob/master/LICENSE)