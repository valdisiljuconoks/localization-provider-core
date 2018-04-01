# Getting Started with AdminUI for Asp.Net Core

## Install Package

```
PM> Install-Package LocalizationProvider.AdminUI.AspNetCore
```

## Configure Services

In order to add AdminUI module to your Asp.Net Core Mvc application you have to first add services to dependency container (service collection) via `services.AddDbLocalizationProviderAdminUI()` method:

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ...

        services.AddDbLocalizationProvider(cfg =>
        {
            cfg...
        });

        services.AddDbLocalizationProviderAdminUI(c =>
        {
            c.ShowInvariantCulture = true;
        });
    }
}
```

You can also configure AdminUI according to your requirements by using passed in UI configuration context.
When you are done with adding of the services, next you need to map AdminUI module under some path (url for the admin part):

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ...
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        ...

        app.UseDbLocalizationProvider();
        app.UseDbLocalizationProviderAdminUI("/admin-ui");
    }
}
```

## Securing Admin UI

TBD
