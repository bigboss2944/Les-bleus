using AspNet_FilRouge.Models;
using AspNet_FilRouge.Services;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;
using System.Text;

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
    options.AccessDeniedPath = "/Account/Login";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.Name = "AspNetFilRougeAdminAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = hasHttpsEndpoint ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// JWT bearer — utilisé par l'application MAUI pour s'authentifier auprès de l'API REST,
// en complément (et non en remplacement) du cookie Identity utilisé par les pages web.
var jwtSigningKey = JwtOptionsProvider.GetSigningKey(builder.Configuration, builder.Environment);
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "FilRouge.Admin";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "FilRouge.Mobile";

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(GetDataProtectionKeysDirectory(builder))
    .SetApplicationName("FilRouge.Admin");

builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(Shared.Controllers.HomeController).Assembly);

builder.Services.AddSingleton<ILocalDbService, LocalDbService>();
builder.Services.AddSingleton<IVendorSyncService, VendorSyncService>();
builder.Services.AddScoped<IOrderPricingService, OrderPricingService>();

builder.Services.AddScoped<IBicycleRepository, BicycleRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IBicycleService, BicycleService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<IAccountService, AccountService>();

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

if (app.Environment.IsDevelopment())
{
    await SeedDemoSellerDataAsync(app.Services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

if (hasHttpsEndpoint)
{
    app.UseHsts();
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
        if (!environment.IsDevelopment() && !environment.IsEnvironment("Testing"))
        {
            throw new InvalidOperationException(
                "SeedUsers:AdminPassword doit être défini hors développement. " +
                "Définissez la variable d'environnement SeedUsers__AdminPassword.");
        }
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

// Jeu de données de démo (Development uniquement) pour tester l'appli MAUI : un compte vendeur,
// un client, un petit catalogue de vélos et trois commandes dans des états différents
// (validée, en cours, vide) afin de couvrir les écrans liste/détail/ajout-retrait de vélo.
static async Task SeedDemoSellerDataAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var sellerService = scope.ServiceProvider.GetRequiredService<ISellerService>();
    var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
    var bicycleService = scope.ServiceProvider.GetRequiredService<IBicycleService>();
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedDemoSellerData");

    const string sellerEmail = "vendeur.demo@filrouge.local";
    const string sellerPassword = "Vendeur!234";

    var sellerUser = await userManager.FindByEmailAsync(sellerEmail);
    if (sellerUser == null)
    {
        var createResult = await sellerService.CreateSellerAccountAsync(new CreateSellerViewModel
        {
            FirstName = "Julie",
            LastName = "Martin",
            Email = sellerEmail,
            Password = sellerPassword,
            Address = "12 rue des Cycles, 75000 Paris",
            Phone = "0600000000"
        });
        if (!createResult.Succeeded)
        {
            logger.LogWarning("Impossible de créer le vendeur de démo : {Errors}",
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return;
        }
        sellerUser = await userManager.FindByEmailAsync(sellerEmail);
    }

    var seller = await sellerService.GetByIdAsync(sellerUser!.Id);
    if (seller == null)
    {
        logger.LogWarning("Compte Identity du vendeur de démo créé mais entité Seller introuvable.");
        return;
    }

    var customers = await customerService.GetAllAsync();
    var customer = customers.FirstOrDefault(c => c.Email == "client.demo@filrouge.local");
    if (customer == null)
    {
        customer = new Customer
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "client.demo@filrouge.local",
            Email = "client.demo@filrouge.local",
            FirstName = "Marc",
            LastName = "Dupont",
            Address = "5 avenue des Fleurs",
            Town = "Lyon",
            PostalCode = 69000,
            Phone = "0601020304"
        };
        await customerService.CreateAsync(customer);
    }

    var availableBicycles = await bicycleService.GetAvailableAsync();
    if (availableBicycles.Count < 5)
    {
        var demoBicycles = new[]
        {
            new Bicycle { TypeOfBike = "VTT", Category = "Adulte", Reference = "VTT-2024-A", FreeTaxPrice = 450f, Tax = 20f, Quantity = 5, Exchangeable = true, Insurance = false, Deliverable = true },
            new Bicycle { TypeOfBike = "Route", Category = "Adulte", Reference = "ROUTE-2024-B", FreeTaxPrice = 890f, Tax = 20f, Quantity = 3, Exchangeable = false, Insurance = true, Deliverable = true },
            new Bicycle { TypeOfBike = "Ville", Category = "Adulte", Reference = "VILLE-2024-C", FreeTaxPrice = 320f, Tax = 20f, Quantity = 4, Exchangeable = true, Insurance = false, Deliverable = false },
            new Bicycle { TypeOfBike = "Électrique", Category = "Adulte", Reference = "VAE-2024-D", FreeTaxPrice = 1590f, Tax = 20f, Quantity = 2, Exchangeable = false, Insurance = true, Deliverable = true },
            new Bicycle { TypeOfBike = "Enfant", Category = "Enfant", Reference = "ENF-2024-E", FreeTaxPrice = 180f, Tax = 20f, Quantity = 6, Exchangeable = true, Insurance = false, Deliverable = true },
        };
        foreach (var bicycle in demoBicycles)
        {
            await bicycleService.CreateAsync(bicycle);
        }
        availableBicycles = await bicycleService.GetAvailableAsync();
    }

    var existingOrders = await orderService.GetPagedAsync(1, 10, seller.Id);
    if (existingOrders.Count == 0 && availableBicycles.Count >= 3)
    {
        var order1 = new Order
        {
            Date = DateTime.UtcNow.AddDays(-2),
            Seller = seller,
            Customer = customer,
            PayMode = "Carte"
        };
        await orderService.CreateAsync(order1);
        await orderService.AddBicycleAsync(order1.IdOrder, availableBicycles[0].Id);
        await orderService.AddBicycleAsync(order1.IdOrder, availableBicycles[1].Id);
        await orderService.ValidateAsync(order1.IdOrder);

        var order2 = new Order
        {
            Date = DateTime.UtcNow.AddDays(-1),
            Seller = seller,
            Customer = customer,
            PayMode = "Espèces"
        };
        await orderService.CreateAsync(order2);
        await orderService.AddBicycleAsync(order2.IdOrder, availableBicycles[2].Id);

        var order3 = new Order
        {
            Date = DateTime.UtcNow,
            Seller = seller
        };
        await orderService.CreateAsync(order3);

        logger.LogInformation("Jeu de données de démo créé pour le vendeur {SellerEmail} : 3 commandes.", sellerEmail);
    }
}
