using IdentityServer.Core.Infrastructure;
using IdentityServer.Data;
using IdentityServer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

string configuration = builder
    .Configuration
    .GetConnectionString("DefaultConnection");

services
    .AddDbContext<IdentityServerDbContext>(config => config
        .UseSqlServer(configuration))
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

string? migrationsAssembly = Assembly
    .GetExecutingAssembly()
    .GetName()
    .Name?
    .Replace("Core", "Data");

services
    .AddIdentityServer()
    .AddAspNetIdentity<IdentityServerUser>()
    .AddConfigurationStore(config => config
        .ConfigureDbContext = options => options
            .UseSqlServer(configuration, sql => sql.MigrationsAssembly(migrationsAssembly)))
    .AddOperationalStore(config => config
        .ConfigureDbContext = options => options
            .UseSqlServer(configuration, sql => sql.MigrationsAssembly(migrationsAssembly)))
    .AddDeveloperSigningCredential();

services
    .AddControllersWithViews();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app
    .UseRouting()
    .UseIdentityServer()
    .UseInitializer()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapDefaultControllerRoute();
    });

app.Run();