using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace LesBleus.Tests.Functional.Helpers;

public class VendeurWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Remove ALL ApplicationDbContext and its options to prevent duplicate provider error
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<DbContextOptions>();

            // Remove background services that require real DB / file system
            var toRemove = services
                .Where(d => d.ImplementationType?.Name is "SyncBackgroundService" or "LocalDbService")
                .ToList();
            foreach (var s in toRemove) services.Remove(s);

            // Re-add DbContext with in-memory database only
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestVendeur_" + Guid.NewGuid())
                       .EnableServiceProviderCaching(false));
        });

        builder.ConfigureLogging(logging => logging.ClearProviders());
    }
}
