# Client-side Localization

Starting with v5.4.1 version of the [LocalizationProvider package for .NET Core](https://www.nuget.org/packages/LocalizationProvider.AspNetCore/), it's now possible to work with translations also on client-side.
In version v6 some of the additional features have been introduced.

## Getting Started

First of all you need to initialize and add client-side resource handler to your application. As usual in .NET Core apps this done via `Startup.cs` file:

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        ...
        app.UseDbLocalizationProvider();
        app.UseDbLocalizationClientsideProvider();
    }
}
```

Next step is to map middleware on particular path (by default `/jsl10n`).
Depending on your routing system (either old school Mvc router or Endpoint routing) different methods needs to be called.

For **old MVC Routing**:

```csharp
public void ConfigureServices(IServiceCollection services)
{
   services
       .AddControllersWithViews(opt => opt.EnableEndpointRouting = false)
       .AddMvcLocalization();
       
   services.AddRouting();
   ...
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    ...
    app.UseMvc(routes =>
    {
        routes.MapDbLocalizationClientsideProvider();

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
   services.AddRouting();
   ...
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseRouting();
    ...
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        ...

        endpoints.MapDbLocalizationClientsideProvider();
    });
}
```

Then in your markup files you can use following helper method to pull down resources.

Assume that we have following resource class:

```csharp
namespace MyProject
{
    [LocalizedResource]
    public class SampleResources
    {
        public static string PageHeader => "This is page header";
    }
}
```

Then by adding this code to your Razor page:

```csharp
<body>
    ...
    @Html.GetTranslations(typeof(SampleResources))
</body>
```

You will get `SampleResources` class pulled down to client-side formatted as JSON object and assigned to `window.jsl10n` object by default. You can work with it in following way:

```javascript
@Html.GetTranslations(typeof(SampleResources))

<script>
    document.getElementById('...').innerHTML = 
            window.jsl10n.MyProject.SampleResources.PageHeader;
</script>
```

Developer experience working with resources on client-side wanted to keep close enough with the same experience when using package on server-side.

You can also fetch *only* required resource keys (without pulling down whole class):

```csharp
@(Html.GetTranslations(() => SampleResources.PageHeader))
```

You can also ask to pull down various resources multiple times. All of the requested resources will be "merged" under this "jsl10n" key on `window` object.

## Aliases

Sometimes you need to assigned different resources to different scopes or somehow group them differently. This is possible by using aliases. What it means is that following code:

```csharp
@Html.GetTranslations(typeof(SampleResources), "no", "norwegianScope")
```

will return resource translations for Norwegian language for `SampleResources` resource under `norwegianScope` key on `window` object level. Sometimes might become handy if needed.

```javascript
var headerInNorwegian = window.norwegianScope.MyProject.SampleResources.PageHeader;
```

## Why `window` Object?

Pulled down translated object needs to be assigned to something in order to work with it. I chose `window` object, but it could be anything actually.

But you might ask - what about case when I need to dynamically pull down resources on my own and don't need to pollute global scope with some weird objects?

## Resources as JSON

In these cases you can issue request directly to localization provider endpoint and get only JSON representation of the resource class translations.

For example, issuing request straight to resource endpoint and providing `json` query parameter, you will get back only JSON object.

![2019-03-09_09-05-30](https://blog.tech-fellow.net/content/images/2019/03/2019-03-09_09-05-30.png)

Or you can just fire any standard XHR request and localization provider will try its best to detect that request is of ajax type. If so - translations will be served in JSON format by default.
