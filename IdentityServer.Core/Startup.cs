namespace IdentityServer.Core
{
    using Data;
    using Data.Models;
    using Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.Reflection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<IdentityServerDbContext>(config =>
                {
                    config.UseInMemoryDatabase(this.Configuration.GetConnectionString("DefaultInMemory"));
                })
                .AddIdentity<IdentityServerUser, IdentityRole>(config =>
                {
                    config.Password.RequiredLength = 4;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<IdentityServerDbContext>()
                .AddDefaultTokenProviders();

            services
                .ConfigureApplicationCookie(config =>
                {
                    config.Cookie.Name = "IdentityServer.IdentityCookie";
                    config.LoginPath = "/Auth/Login";
                });

            var migrationsAssembly = typeof(Startup)
                .GetTypeInfo()
                .Assembly
                .GetName().Name
                .Replace("Core", "Data");

            services
                .AddIdentityServer()
                .AddAspNetIdentity<IdentityServerUser>()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddInMemoryClients(Config.GetClients())
                //.AddConfigurationStore(config =>
                //{
                //    config.ConfigureDbContext = options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"),
                //        sql => sql.MigrationsAssembly(migrationsAssembly));
                //})
                //.AddOperationalStore(config =>
                //{
                //    config.ConfigureDbContext = options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"),
                //        sql => sql.MigrationsAssembly(migrationsAssembly));
                //})
                .AddDeveloperSigningCredential();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();

            //app.UseInitializer(env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}