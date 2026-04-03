using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace AspNet_FilRouge.Tests.Functional.Helpers;

public class AdminWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<DbContextOptions>();

            var toRemove = services
                .Where(d => d.ImplementationType?.Name is "SyncBackgroundService" or "LocalDbService")
                .ToList();
            foreach (var s in toRemove) services.Remove(s);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestAdmin_" + Guid.NewGuid())
                       .EnableServiceProviderCaching(false));
        });

        builder.ConfigureLogging(logging => logging.ClearProviders());
    }
}
