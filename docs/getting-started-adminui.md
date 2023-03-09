# Getting Started with AdminUI for Asp.Net Core

## Install Package

```
> dotnet add package LocalizationProvider.AdminUI.AspNetCore
```

## Configure Services

In order to add AdminUI module to your Asp.Net Core Mvc application you have to first add services to dependency container (service collection) via `services.AddDbLocalizationProviderAdminUI()` method:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllersWithViews()
            .AddMvcLocalization();
    
        services.AddRazorPages();
        services.AddRouting();

        ...

        services.AddDbLocalizationProvider(cfg =>
        {
            // configure provider
            // for example cfg.UseSqlServer(...);
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

You can also configure AdminUI according to your requirements by using passed in UI configuration context (`UiConfigurationContext`).

Following configuration options are available:

| Option | Description |
|------|------|
| `MaxResourceKeyDisplayLength` | Maximnum number of symbols to show for resource key. Default 80.  |
| `MaxResourceKeyPopupTitleLength` | Maximnum number of symbols to show for resource edit window title. Default 80. |
| `DefaultView` | Which view to show when accessing AdminUI. Default is `Table`. |
| `ShowInvariantCulture` | Do you want to see invariant culture column? |
| `ShowHiddenResources` | Do you want to see hidden resources (decorated with `[Hidden]` attribute)? |
| `CustomCssPath` | Make your AdminUI look familiar using external CSS file. |
| `RootUrl` | Mapping url of AdminUI (by which URL you will be able to access the user interface). |
| `HideDeleteButton` | Should `Delete` button be visible? |
| `AccessPolicyOptions` | How are you going to secure access to AdminUI? |
| `UseAvailableLanguageListFromStorage` | Flag whether list of available languages should be taked from the underlying storage. |

When you are done with adding these services, next you need to map AdminUI module under some path.

Starting from v6 there are few changes how AdminUI is mapped.

For **old MVC Routing**:

```csharp
public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDbLocalizationProvider();
        app.UseDbLocalizationProviderAdminUI();

        ...
        app.UseMvc(routes =>
        {
            routes.MapDbLocalizationAdminUI();
        });
    }
}
```

For **Endpoint routing**:

```csharp
public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDbLocalizationProvider();
        app.UseDbLocalizationProviderAdminUI();

        ...

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
            ...
        });
    }
}
```

## Accessing AdminUI

By default administration UI is mapped on `/localization-admin` path. You can customize path via `app.AddDbLocalizationProviderAdminUI();`. For example to map to `/loc-admin-ui`, you have to:

```
public class Startup
{
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

AdminUI by default is secured with user roles access policy requirement.

```csharp
public class CheckAdministratorsRoleRequirement
    : AuthorizationHandler<CheckAdministratorsRoleRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CheckAdministratorsRoleRequirement requirement)
    {
        if (context.User.IsInRole("Administrators"))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
```
If you want to customize access policy - you can configure it via `Configure` method on startup:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddDbLocalizationProviderAdminUI(_ =>
        {
            _.AccessPolicyOptions = builder => builder.AddRequirements(...);
        });
}
```
