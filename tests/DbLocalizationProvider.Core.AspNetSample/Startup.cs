using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.AdminUI.AspNetCore;
using DbLocalizationProvider.AdminUI.AspNetCore.Routing;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.AspNetCore.ClientsideProvider;
using DbLocalizationProvider.AspNetCore.ClientsideProvider.Routing;
using DbLocalizationProvider.AspNetCore.EntityFramework.Extensions;
using DbLocalizationProvider.AspNetCore.Extensions;
using DbLocalizationProvider.Core.AspNet.ForeignAssembly;
using DbLocalizationProvider.Core.AspNetSample.Data;
using DbLocalizationProvider.Core.AspNetSample.Extensions;
using DbLocalizationProvider.Core.AspNetSample.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                options =>
                {
                   // options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection"));
                   options.UseNpgsql(Configuration.GetConnectionString("PostgreSqlConnection"));
                   options.UseLocalizationProvider();
                });

            services
                .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // mvc stuff
            services
                .AddControllersWithViews( /*opt => opt.EnableEndpointRouting = false*/ )
                .AddMvcLocalization();

            services.AddRazorPages();
            services.AddRouting();
            services.AddHealthChecks();

            services.AddLogging(c =>
            {
                c.AddConsole();
                c.AddDebug();
            });

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
                _.DefaultResourceCulture = CultureInfo.InvariantCulture;
                _.CustomAttributes.Add(typeof(WeirdCustomAttribute));
                _.ScanAllAssemblies = true;
                _.FallbackCultures.Try(supportedCultures);
                _.ForeignResources.Add<SomeForeignViewModel>();
                //.Try(new CultureInfo("sv"))
                //.Then(new CultureInfo("no"))
                //.Then(new CultureInfo("en"));
                _.UseEntityFramework<ApplicationDbContext>();
            });

            services.AddDbLocalizationProviderAdminUI(_ =>
            {
                _.RootUrl = "/localization-admin";

                _.AuthorizedAdminRoles.Clear();
                _.AuthorizedAdminRoles.Add("Administrators");

                _.AuthorizedEditorRoles.Clear();
                _.AuthorizedEditorRoles.Add("Translators");

                _.ShowInvariantCulture = true;
                _.ShowHiddenResources = false;
                _.DefaultView = ResourceListView.Tree;
                _.CustomCssPath = "/css/custom-adminui.css";
                _.HideDeleteButton = false;
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

            app.ApplyMigrations();
            app.SeedUserData("test@test.com","Test@123", "Administrators");

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

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
            }

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
