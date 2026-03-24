using AspNet_FilRouge.Models;
using AspNet_FilRouge.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);
var hasHttpsEndpoint = HasHttpsEndpoint(builder.Configuration);

// Database - shared with vendor app
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=aspnet-AspNet_FilRouge;Trusted_Connection=True;MultipleActiveResultSets=true";

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    var sqliteConnectionString = builder.Configuration.GetConnectionString("SqliteConnection")
        ?? "Data Source=aspnet-filrouge.db";
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(sqliteConnectionString));
}

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<LocalDbService>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

builder.Services.AddHostedService<SyncBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    await EnsureSqliteIdentitySchemaAsync(dbContext);
    await EnsureSqliteStockRequestsTableAsync(dbContext);
}

await SeedDefaultAdminAsync(app.Services, app.Configuration);

if (!app.Environment.IsDevelopment() && hasHttpsEndpoint)
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (hasHttpsEndpoint)
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Scripts")),
    RequestPath = "/Scripts"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Content")),
    RequestPath = "/Content"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Pictures")),
    RequestPath = "/Pictures"
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task EnsureSqliteIdentitySchemaAsync(ApplicationDbContext dbContext)
{
    if (!dbContext.Database.IsSqlite())
    {
        return;
    }

    var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    var connection = dbContext.Database.GetDbConnection();
    var wasClosed = connection.State != System.Data.ConnectionState.Open;

    if (wasClosed)
    {
        await connection.OpenAsync();
    }

    try
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA table_info('AspNetUsers');";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns.Add(reader.GetString(1));
        }
    }
    finally
    {
        if (wasClosed)
        {
            await connection.CloseAsync();
        }
    }

    var alterStatements = new List<string>();

    if (!columns.Contains("FirstName"))
    {
        alterStatements.Add("ALTER TABLE AspNetUsers ADD COLUMN FirstName TEXT NULL;");
    }

    if (!columns.Contains("LastName"))
    {
        alterStatements.Add("ALTER TABLE AspNetUsers ADD COLUMN LastName TEXT NULL;");
    }

    if (!columns.Contains("Address"))
    {
        alterStatements.Add("ALTER TABLE AspNetUsers ADD COLUMN Address TEXT NULL;");
    }

    foreach (var alterStatement in alterStatements)
    {
        await dbContext.Database.ExecuteSqlRawAsync(alterStatement);
    }
}

static async Task EnsureSqliteStockRequestsTableAsync(ApplicationDbContext dbContext)
{
    if (!dbContext.Database.IsSqlite())
    {
        return;
    }

    var connection = dbContext.Database.GetDbConnection();
    var wasClosed = connection.State != System.Data.ConnectionState.Open;

    if (wasClosed)
    {
        await connection.OpenAsync();
    }

    try
    {
        // Table name is a hardcoded constant — no user input, no injection risk.
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='StockRequests';";
        var result = await command.ExecuteScalarAsync();
        if (result == null)
        {
            // Schema must match the StockRequest model in Models/IdentityModels.cs.
            // AspNetUsers is guaranteed to exist: EnsureCreated() runs before this method.
            await using var createCommand = connection.CreateCommand();
            createCommand.CommandText = @"
                CREATE TABLE StockRequests (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    BicycleName TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    RequestDate TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    RequestedById TEXT,
                    Notes TEXT,
                    FOREIGN KEY (RequestedById) REFERENCES AspNetUsers(Id)
                );";
            await createCommand.ExecuteNonQueryAsync();
        }
    }
    finally
    {
        if (wasClosed)
        {
            await connection.CloseAsync();
        }
    }
}

static bool HasHttpsEndpoint(ConfigurationManager configuration)
{
    var urls = configuration["ASPNETCORE_URLS"]
        ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

    if (string.IsNullOrWhiteSpace(urls))
    {
        return false;
    }

    return urls
        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
}

static async Task SeedDefaultAdminAsync(IServiceProvider services, IConfiguration configuration)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    const string adminRole = "Administrateur";
    const string vendeurRole = "Vendeur";

    var adminUserName = configuration["SeedCredentials:AdminUserName"] ?? "admin";
    var adminEmail = configuration["SeedCredentials:AdminEmail"] ?? "admin@filrouge.local";
    var adminPassword = configuration["SeedCredentials:AdminPassword"] ?? "Admin!234";

    foreach (var roleName in new[] { adminRole, vendeurRole })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new ApplicationRole(roleName));
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Unable to create {roleName} role.");
            }
        }
    }

    var adminUser = await userManager.FindByNameAsync(adminUserName)
                    ?? await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminUserName,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var userResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (!userResult.Succeeded)
        {
            throw new InvalidOperationException("Unable to create default admin user.");
        }
    }

    if (!await userManager.IsInRoleAsync(adminUser, adminRole))
    {
        var addToRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
        if (!addToRoleResult.Succeeded)
        {
            throw new InvalidOperationException("Unable to assign Administrateur role to default user.");
        }
    }
}
