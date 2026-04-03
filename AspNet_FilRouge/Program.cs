using AspNet_FilRouge.Models;
using AspNet_FilRouge.Services;
using Microsoft.Data.Sqlite;
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
    var configuredSharedDbPath = builder.Configuration["Sync:SharedDbPath"];
    var sharedDbPath = string.IsNullOrWhiteSpace(configuredSharedDbPath)
        ? Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "aspnet-filrouge.shared.db"))
        : (Path.IsPathRooted(configuredSharedDbPath)
            ? configuredSharedDbPath
            : Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, configuredSharedDbPath)));

    var sharedDbDirectory = Path.GetDirectoryName(sharedDbPath);
    if (!string.IsNullOrWhiteSpace(sharedDbDirectory))
    {
        Directory.CreateDirectory(sharedDbDirectory);
    }

    var sqliteConnectionString = $"Data Source={sharedDbPath}";
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
    options.Cookie.Name = "AspNetFilRougeAdminAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = hasHttpsEndpoint ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(GetDataProtectionKeysDirectory(builder))
    .SetApplicationName("FilRouge");

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<ILocalDbService, LocalDbService>();
builder.Services.AddSingleton<IVendorSyncService, VendorSyncService>();
builder.Services.AddScoped<IOrderPricingService, OrderPricingService>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

builder.Services.AddHostedService<SyncBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    EnsureCreatedWithSqliteRaceTolerance(dbContext);
    await EnsureSqliteIdentitySchemaAsync(dbContext);
    await EnsureSqliteBicyclesSchemaAsync(dbContext);
    await EnsureSqliteStockRequestsTableAsync(dbContext);
}

await SeedDefaultAdminAsync(app.Services, app.Configuration, app.Environment);

if (!app.Environment.IsDevelopment() && hasHttpsEndpoint)
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (hasHttpsEndpoint)
{
    app.UseHttpsRedirection();
}
// Les fichiers statiques sont désormais servis depuis wwwroot (par défaut)
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void EnsureCreatedWithSqliteRaceTolerance(ApplicationDbContext dbContext)
{
    try
    {
        dbContext.Database.EnsureCreated();
    }
    catch (SqliteException ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
    {
        // Two app instances can call EnsureCreated concurrently on the shared SQLite file.
        // If a table was created by the other process between checks, we can safely continue.
    }
}

static System.IO.DirectoryInfo GetDataProtectionKeysDirectory(WebApplicationBuilder builder)
{
    var configuredPath = builder.Configuration["DataProtection:KeysPath"];
    string keysPath;

    if (!string.IsNullOrWhiteSpace(configuredPath))
    {
        keysPath = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, configuredPath));
    }
    else if (builder.Environment.IsProduction())
    {
        keysPath = "/data/keys";
    }
    else
    {
        keysPath = Path.Combine(Path.GetTempPath(), "filrouge", builder.Environment.ApplicationName, "keys");
    }

    Directory.CreateDirectory(keysPath);
    return new System.IO.DirectoryInfo(keysPath);
}

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

static async Task EnsureSqliteBicyclesSchemaAsync(ApplicationDbContext dbContext)
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
        command.CommandText = "PRAGMA table_info('Bicycles');";

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

    if (!columns.Contains("Quantity"))
    {
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Bicycles ADD COLUMN Quantity INTEGER NOT NULL DEFAULT 1;");
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

static async Task SeedDefaultAdminAsync(IServiceProvider services, IConfiguration configuration, IWebHostEnvironment environment)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedDefaultAdmin");

    const string adminRole = AppConstants.Roles.Administrateur;
    const string vendeurRole = AppConstants.Roles.Vendeur;
    const string adminUserName = "admin";
    const string adminEmail = "admin@filrouge.local";
    // Lire le mot de passe depuis la configuration (variable d'environnement SeedUsers__AdminPassword ou appsettings).
    var configuredPassword = configuration["SeedUsers:AdminPassword"];
    if (string.IsNullOrWhiteSpace(configuredPassword))
    {
        if (environment.IsProduction())
            throw new InvalidOperationException(
                "SeedUsers:AdminPassword doit être défini en production. " +
                "Définissez la variable d'environnement SeedUsers__AdminPassword.");
        logger.LogWarning("SeedUsers:AdminPassword n'est pas configuré. Utilisation du mot de passe par défaut (développement uniquement).");
    }
    var adminPassword = configuredPassword ?? "Admin!234";

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
