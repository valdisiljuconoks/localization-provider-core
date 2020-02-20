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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using IApplicationBuilderExtensions = DbLocalizationProvider.AdminUI.AspNetCore.IApplicationBuilderExtensions;

namespace DbLocalizationProvider.Core.AspNetSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services
                .AddControllersWithViews( /*opt => opt.EnableEndpointRouting = false*/ )
                .AddMvcLocalization();

            services.AddAuthorization();
            services.AddRazorPages();
            services.AddRouting();
            services.AddHealthChecks();

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
                _.CustomAttributes.Add(typeof(WeirdCustomAttribute));
                _.ScanAllAssemblies = true;
                _.FallbackCultures.Try(supportedCultures);

                //.Try(new CultureInfo("sv"))
                //.Then(new CultureInfo("no"))
                //.Then(new CultureInfo("en"));

                _.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDbLocalizationProviderAdminUI(_ =>
            {
                _.RootUrl = "/localization-admin";
                _.AuthorizedAdminRoles.Add("Admin");
                _.ShowInvariantCulture = true;
                _.ShowHiddenResources = false;
                _.DefaultView = ResourceListView.Tree;
                _.CustomCssPath = "/css/custom-adminui.css";
            });

            //.VerifyDbLocalizationProviderAdminUISetup();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            // app.UseMvc(routes =>
            // {
            //     routes.MapDbLocalizationAdminUI();
            //     routes.MapDbLocalizationClientsideProvider();
            //
            //     routes.MapRoute(
            //         name: "default",
            //         template: "{controller=Home}/{action=Index}/{id?}");
            // });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                endpoints.MapDbLocalizationAdminUI();
                endpoints.MapDbLocalizationClientsideProvider();

                endpoints.MapHealthChecks("healthz");
            });


            // app.UseMvc(routes =>
            //                // Enables HTML5 history mode for Vue app
            //                // Ref: https://weblog.west-wind.com/posts/2017/aug/07/handling-html5-client-route-fallbacks-in-aspnet-core
            //                routes.MapRoute(
            //                    name: "catch-all",
            //                    template: "{*url}",
            //                    defaults: new { controller = "Home", action = "Index" }));
        }
    }
}
