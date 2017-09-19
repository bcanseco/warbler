using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Warbler.Hubs;
using Warbler.Models;
using Warbler.Misc;
using Warbler.Interfaces;
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

        public static void Main(string[] args) => BuildWebHost(args).Run();

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
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
                options.LoginPath = "/Account/LogIn";
                options.ExpireTimeSpan = TimeSpan.FromDays(150);
            });

            services.AddAuthentication();

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
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles(); // wwwroot (node_modules)

            var folders = Directory.GetDirectories(".", "Scripts", SearchOption.AllDirectories)
                .Concat(Directory.GetDirectories(".", "Styles", SearchOption.AllDirectories))
                .Where(f => !f.Contains("node_modules"))
                .Select(f => f.Substring(1, f.Length - 1).Replace(@"\", "/")).ToList();
            
            folders.Add("/Graphics"); // Winter's logos

            foreach (var folder in folders)
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        $"{Directory.GetCurrentDirectory()}{folder}"),
                    RequestPath = new PathString(folder)
                });
            }

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
            
            if (env.IsDevelopment())
            {
                var context = serviceProvider.GetService<WarblerDbContext>();
                context.Database.EnsureCreated();
            }
        }
    }
}
