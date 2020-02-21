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
            ...
            c.ShowInvariantCulture = true;
        });
    }
}
```

You can also configure AdminUI according to your requirements by using passed in UI configuration context.
When you are done with adding of the services, next you need to map AdminUI module under path:
Starting from v6 there are changes now AdminUI is mapped.

For **old MVC Routing**:

```csharp
public void ConfigureServices(IServiceCollection services)
{
   services
       .AddControllersWithViews(opt => opt.EnableEndpointRouting = false)
       .AddMvcLocalization();

    services.AddRouting();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    ...
    app.UseMvc(routes =>
    {
        routes.MapDbLocalizationAdminUI();

        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });
}
```

For **Endpoint routing**:

```csharp
public void ConfigureServices(IServiceCollection services)
{
   services
       .AddControllersWithViews()
       .AddMvcLocalization();

   services.AddRouting();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseRouting();
    ...
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        ...
        
        endpoints.MapDbLocalizationAdminUI();
    });
}
```

## Accessing AdminUI

By default administration UI is mapped on `/localization-admin` path. You can customize path via `app.AddDbLocalizationProviderAdminUI();`. For example to map to `/loc-admin-ui`, you have to:

```
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbLocalizationProviderAdminUI(_ =>
        {
            _.RootUrl = "/loc-admin-ui";
        });
    }
}
```

## Securing Admin UI

AdminUI by default is secured via roles which you can configure yourself via `Configure` method on startup:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbLocalizationProviderAdminUI(_ =>
    {
        ...
        _.AuthorizedAdminRoles.Add("Admins");
        _.AuthorizedEditorRoles.Add("Translators");
    });
}
```

In order for you to get this working, you need to enable roles based access in your ASP.NET identity setup:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<...>(...);
    
    services
        .AddDefaultIdentity<...>(...)
        .AddRoles<IdentityRole>();
}
```
