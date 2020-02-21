# LocalizationProvider for .NET Core

Database driven localization provider for .NET Core applications.

[<img src="https://tech-fellow-consulting.visualstudio.com/_apis/public/build/definitions/f63fd8ab-e3f1-48c1-bca0-f027727a53c4/9/badge"/>](https://tech-fellow-consulting.visualstudio.com/localization-provider-core/_build/index?definitionId=9)

## What is the LocalizationProvider project?

LocalizationProvider project is Asp.Net Mvc web application localization provider on steriods.

Giving you main following features:
* Database driven localization provider for .Net applications
* Easy resource registrations via code
* Supports hierarchical resources (with help of child classes)

## What's new in v6?
Please [refer to this post]() to read more about new features in v6.

## Project Structure

Database localization provider is split into main [abstraction projects](https://github.com/valdisiljuconoks/LocalizationProvider) and .NET Core support project (this).

## Getting Started

### Bare Minimum to Start With
Below are code fragments that are essential to get started with localization provider.

Install required packages:

```
> dotnet add package LocalizationProvider.AdminUI.AspNetCore
> dotnet add package LocalizationProvider.Storage.SqlServer
```

Following `Startup.cs` file seems to be working OK as starting point for your app (based ASP.NET Core 3.1):

```csharp
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.AdminUI.AspNetCore;
using DbLocalizationProvider.AdminUI.AspNetCore.Routing;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.AspNetCore.ClientsideProvider.Routing;
using DbLocalizationProvider.Core.AspNetSample.Data;
using DbLocalizationProvider.Core.AspNetSample.Resources;
using DbLocalizationProvider.Storage.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services
                .AddControllersWithViews()
                .AddMvcLocalization();

            services.AddAuthorization();
            services.AddRazorPages();
            services.AddRouting();

            var supportedCultures = new List<CultureInfo> { new CultureInfo("sv"), new CultureInfo("no"), new CultureInfo("en") };

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                opts.DefaultRequestCulture = new RequestCulture("en");
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
            });

            services.AddDbLocalizationProvider(_ =>
            {
                _.EnableInvariantCultureFallback = true;
                _.ScanAllAssemblies = true;
                _.FallbackCultures.Try(supportedCultures);
                _.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDbLocalizationProviderAdminUI(_ =>
            {
                _.RootUrl = "/localization-admin";
                _.ShowInvariantCulture = true;
                _.ShowHiddenResources = false;
                _.DefaultView = ResourceListView.Tree;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDbLocalizationProvider();
            app.UseDbLocalizationProviderAdminUI();
            app.UseDbLocalizationClientsideProvider();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                endpoints.MapDbLocalizationAdminUI();
                endpoints.MapDbLocalizationClientsideProvider();
            });
        }
    }
}
```

Also you can refer to [sample app in GitHub](https://github.com/valdisiljuconoks/localization-provider-core/tree/master/tests/DbLocalizationProvider.Core.AspNetSample) for some more hints if needed.

### More Detailed Help

* [Getting Started](docs/getting-started-netcore.md)
* [Getting Started with AdminUI](docs/getting-started-adminui.md)
* [Localizing App Content](docs/localizing-content-netcore.md)
* [Localizing View Model (with DataAnnotations attributes)](docs/localizing-viewmodel-netcore.md)
* [Localizing Client-side](docs/client-side-provider-netcore.md)

## GitHub Source Code Structure

.NET Core support project has its own repo while main abstraction projects are included as [submodules](https://gist.github.com/gitaarik/8735255) here.

# How to Contribute

It's super cool if you read this section and are interesed how to help the library. Forking and playing around sample application is the fastest way to understand how localization provider is working and how to get started.

Forking and cloning repo is first step you do. Keep in mind that provider is split into couple repositories to keep thigns separated. Additional repos are pulled in as submodules. If you Git client does not support automatic checkout of the submodules, just execute this command at the root of the checkout directory:

```
git clone --recurse-submodules git://github.com/...
```

# More Info

* [Part 1: Resources and Models](http://blog.tech-fellow.net/2016/03/16/db-localization-provider-part-1-resources-and-models/)
* [Part 2: Configuration and Extensions](http://blog.tech-fellow.net/2016/04/21/db-localization-provider-part-2-configuration-and-extensions/)
* [Part 3: Import and Export](http://blog.tech-fellow.net/2017/02/22/localization-provider-import-and-export-merge/)
* [Part 4: Resource Refactoring and Migrations](https://blog.tech-fellow.net/2017/10/10/localizationprovider-tree-view-export-and-migrations/)
