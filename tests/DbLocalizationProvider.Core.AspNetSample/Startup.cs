using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.AdminUI.AspNetCore;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.Core.AspNetSample.Data;
using DbLocalizationProvider.Core.AspNetSample.Models;
using DbLocalizationProvider.Core.AspNetSample.Resources;
using DbLocalizationProvider.Core.AspNetSample.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(opts =>
                                                           {
                                                               var supportedCultures = new List<CultureInfo>
                                                                                       {
                                                                                           new CultureInfo("en"),
                                                                                           new CultureInfo("no")
                                                                                       };

                                                               opts.DefaultRequestCulture = new RequestCulture("en");
                                                               opts.SupportedCultures = supportedCultures;
                                                               opts.SupportedUICultures = supportedCultures;
                                                           });

            services.AddDbLocalizationProvider(_ =>
                                               {
                                                   _.EnableInvariantCultureFallback = true;
                                                   _.CustomAttributes.Add(typeof(WeirdCustomAttribute));
                                               });
            services.AddDbLocalizationProviderAdminUI(_ =>
                                                      {
                                                          _.RootUrl = "/localization-admin";
                                                          _.AuthorizedAdminRoles.Add("Admin");
                                                          _.ShowInvariantCulture = true;
                                                          _.ShowHiddenResources = false;
                                                          _.CustomCssPath = "/css/custom-adminui.css";
                                                      });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
                       {
                           routes.MapRoute(
                                           name: "default",
                                           template: "{controller=Home}/{action=Index}/{id?}");
                       });

            app.UseDbLocalizationProvider();
            app.UseDbLocalizationProviderAdminUI();
        }
    }
}
