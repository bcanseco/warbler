using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Warbler.Areas.Chat.Data;
using Warbler.Identity.Data;
using Warbler.Identity.Models;
using Warbler.Identity.Services;

namespace Warbler
{
    public class Startup
    {
        public static IConnectionManager ConnectionManager;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });

            // Add Entity Framework databases.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("WarblerProduction")));
            services.AddDbContext<ChatContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("WarblerDevelopment")));

            // Set up authentication.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 7;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // Cookie settings
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(150);
                options.Cookies.ApplicationCookie.LoginPath = "/Account/Login";

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, ChatContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

            app.UseIdentity();
            app.UseWebSockets();
            app.UseSignalR();

            ConnectionManager = serviceProvider.GetService<IConnectionManager>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DbInitializer.Initialize(context);
        }
    }
}
