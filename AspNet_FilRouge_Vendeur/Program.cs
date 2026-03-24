using AspNet_FilRouge_Vendeur.Models;
using AspNet_FilRouge_Vendeur.Services;
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Static files - use the client app's assets
var clientContentRoot = Path.Combine(app.Environment.ContentRootPath, "..", "AspNet_FilRouge");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(Path.Combine(clientContentRoot, "Scripts"))),
    RequestPath = "/Scripts"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(Path.Combine(clientContentRoot, "Content"))),
    RequestPath = "/Content"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(Path.Combine(clientContentRoot, "Pictures"))),
    RequestPath = "/Pictures"
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed roles and default admin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

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
}

app.Run();
