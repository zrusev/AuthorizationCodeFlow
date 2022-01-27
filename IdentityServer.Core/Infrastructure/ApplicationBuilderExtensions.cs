namespace IdentityServer.Core.Infrastructure
{
    using IdentityServer.Data;
    using IdentityServer4.EntityFramework.DbContexts;
    using IdentityServer4.EntityFramework.Mappers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseInitializer(this IApplicationBuilder app,
           IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database
                    .Migrate();

                var configContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configContext.Database
                    .Migrate();

                if (!configContext.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        configContext.Clients.Add(client.ToEntity());
                    }
                    configContext.SaveChanges();
                }

                if (!configContext.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        configContext.IdentityResources.Add(resource.ToEntity());
                    }
                    configContext.SaveChanges();
                }

                if (!configContext.ApiScopes.Any())
                {
                    foreach (var resource in Config.GetApiScopes())
                    {
                        configContext.ApiScopes.Add(resource.ToEntity());
                    }
                    configContext.SaveChanges();
                }

                var identityContext = serviceScope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();
                identityContext.Database
                    .Migrate();
            }

            return app;
        }
    }
}
