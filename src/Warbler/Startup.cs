using System;
using System.Collections.Generic;
using ComponentSpace.Saml2.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Warbler.Hubs;
using Warbler.Models;
using Warbler.Misc;
using Warbler.Interfaces;
using Warbler.Repositories;
using Warbler.Services;

namespace Warbler
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; }

        public static void Main(string[] args)
            => BuildWebHost(args).Run();

        private static IWebHost BuildWebHost(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc();

            services.Configure<ApiKeys>(Configuration.GetSection(nameof(ApiKeys)));

            services.AddSingleton<ProximityService>();
            services.AddSingleton<ChatService>();

            services.AddSignalR(options =>
            {
                options.JsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
            });

#if DEBUG
            var dbConnection = Configuration.GetConnectionString("WarblerDevelopment");
#else
            var dbConnection = Configuration.GetConnectionString("WarblerProduction");
#endif

            // Add Entity Framework databases.
            services.AddDbContext<WarblerDbContext>(options => options.UseSqlServer(dbConnection));

            // Set up authentication.
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<WarblerDbContext>()
                .AddDefaultTokenProviders();

            // Set up Google authentication. (currently using SAML SSO)
            //services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //});

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.Name = "Warbler.Identity";
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.SlidingExpiration = true;
            });

            // Register the SAML configuration.
            services.Configure<SamlConfigurations>(ConfigureSaml);

            // Add SAML SSO services.
            services.AddSaml();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            var dbContext = serviceProvider.GetService<WarblerDbContext>();
            dbContext.Database.EnsureCreated();

            var samlConfigs = serviceProvider.GetService<SamlConfigurations>();
            var authConfigService = new AuthConfigService(new SqlAuthConfigRepository(dbContext), samlConfigs);
            authConfigService.RefreshConfigsAsync().Wait();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseWebSockets();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ProximityHub>(nameof(ProximityHub));
                routes.MapHub<ChatHub>(nameof(ChatHub));
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void ConfigureSaml(SamlConfigurations samlConfigurations)
        {
            samlConfigurations.Configurations = new List<SamlConfiguration>
            {
                new SamlConfiguration
                {
                    ID = "Default",
                    LocalServiceProviderConfiguration = new LocalServiceProviderConfiguration
                    {
#if DEBUG
                        Name = "https://localhost:44395",
                        Description = "Warbler Development",
                        AssertionConsumerServiceUrl = "https://localhost:44395/SAML/AssertionConsumerService"
#else
                        Name = "https://warblerapp.azurewebsites.net",
                        Description = "Warbler Production",
                        AssertionConsumerServiceUrl = "https://warblerapp.azurewebsites.net/SAML/AssertionConsumerService"
#endif
                    }
                }
            };
        }
    }
}
