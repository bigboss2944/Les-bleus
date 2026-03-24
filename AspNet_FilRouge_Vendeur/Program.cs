using AspNet_FilRouge_Vendeur.Models;
using AspNet_FilRouge_Vendeur.Services;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Database - shared with client app
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\aspnet-AspNet_FilRouge.mdf;Initial Catalog=aspnet-AspNet_FilRouge;Integrated Security=True";

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
    options.LogoutPath = "/Account/LogOff";
    options.AccessDeniedPath = "/Account/Login";
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

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    EnsureCreatedWithSqliteRaceTolerance(dbContext);
    await EnsureSqliteBicyclesSchemaAsync(dbContext);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Static files - use the client app's assets
var clientContentRoot = Path.Combine(app.Environment.ContentRootPath, "..", "AspNet_FilRouge");
var staticDirs = new[] { ("Scripts", "/Scripts"), ("Content", "/Content"), ("Pictures", "/Pictures") };
foreach (var (dir, requestPath) in staticDirs)
{
    var fullPath = Path.GetFullPath(Path.Combine(clientContentRoot, dir));
    if (Directory.Exists(fullPath))
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fullPath),
            RequestPath = requestPath
        });
    }
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed roles and default admin (skipped in test environment)
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    string[] roles = { "Administrateur", "Vendeur" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new ApplicationRole(role));
    }

    const string adminEmail = "admin@lesbleus.fr";
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "Principal",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, "Admin@123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Administrateur");
    }

    const string vendeurEmail = "vendeur@lesbleus.fr";
    var vendeur = await userManager.FindByEmailAsync(vendeurEmail);
    if (vendeur == null)
    {
        vendeur = new ApplicationUser
        {
            UserName = vendeurEmail,
            Email = vendeurEmail,
            FirstName = "Vendeur",
            LastName = "Défaut",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(vendeur, "Vendeur@123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(vendeur, "Vendeur");
    }

    if (vendeur != null && !await dbContext.Sellers.AnyAsync(s => s.Id == vendeur.Id))
    {
        dbContext.Sellers.Add(new Seller
        {
            Id = vendeur.Id,
            UserName = vendeur.UserName,
            Email = vendeur.Email,
            FirstName = vendeur.FirstName,
            LastName = vendeur.LastName,
            PhoneNumber = vendeur.PhoneNumber
        });
        await dbContext.SaveChangesAsync();
    }
}

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
