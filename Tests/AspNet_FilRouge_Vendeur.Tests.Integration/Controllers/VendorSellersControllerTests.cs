using AspNet_FilRouge_Vendeur.Controllers;
using AspNet_FilRouge_Vendeur.Models;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LesBleus.Tests.Integration.Controllers;

public class VendorSellersControllerTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        return userManager;
    }

    private static SellersController CreateController(ApplicationDbContext context, Mock<UserManager<ApplicationUser>>? userManagerMock = null)
    {
        var userManager = userManagerMock ?? CreateUserManagerMock();
        var controller = new SellersController(context, userManager.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal(
                    new System.Security.Claims.ClaimsIdentity(
                        new[] 
                        { 
                            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "admin"),
                            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Administrateur")
                        }, "test"))
            }
        };
        return controller;
    }

    [Fact]
    public async Task Index_ReturnsViewWithAllSellers()
    {
        // Arrange
        using var context = CreateContext();
        var sellers = new[]
        {
            new Seller { Id = "1", FirstName = "John", LastName = "Doe", Email = "john@shop.com" },
            new Seller { Id = "2", FirstName = "Jane", LastName = "Smith", Email = "jane@shop.com" }
        };
        context.Sellers.AddRange(sellers);
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedSellers = Assert.IsAssignableFrom<List<Seller>>(viewResult.Model);
        Assert.Equal(2, returnedSellers.Count);
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithSeller()
    {
        // Arrange
        using var context = CreateContext();
        var seller = new Seller { Id = "1", FirstName = "John", LastName = "Doe", Email = "john@shop.com" };
        context.Sellers.Add(seller);
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.Details("1");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedSeller = Assert.IsType<Seller>(viewResult.Model);
        Assert.Equal("1", returnedSeller.Id);
        Assert.Equal("John", returnedSeller.FirstName);
    }

    [Fact]
    public async Task Details_WithNullId_ReturnsBadRequest()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);

        // Act
        var result = await controller.Details(null);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);

        // Act
        var result = await controller.Details("nonexistent");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);

        // Act
        var result = controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_CreatesSeller()
    {
        // Arrange
        using var context = CreateContext();
        var userManagerMock = CreateUserManagerMock();
        var newUser = new ApplicationUser { Id = "newid", Email = "newvendor@shop.com", FirstName = "New", LastName = "Vendor" };
        
        userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .Callback<ApplicationUser, string>((user, pwd) => 
            {
                user.Id = newUser.Id;
                context.Users.Add(user);
            })
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var controller = CreateController(context, userManagerMock);
        var model = new CreateSellerViewModel
        {
            Email = "newvendor@shop.com",
            FirstName = "New",
            LastName = "Vendor",
            Address = "1 Test Street",
            Phone = "0102030405",
            Password = "SecurePass123!"
        };

        // Act
        var result = await controller.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithInvalidModel_ReturnsView()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);
        controller.ModelState.AddModelError("Email", "Required");
        var model = new CreateSellerViewModel();

        // Act
        var result = await controller.Create(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Edit_Get_WithValidId_ReturnsViewWithSeller()
    {
        // Arrange
        using var context = CreateContext();
        var seller = new Seller { Id = "1", FirstName = "John", LastName = "Doe" };
        context.Sellers.Add(seller);
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.Edit("1");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedSeller = Assert.IsType<Seller>(viewResult.Model);
        Assert.Equal("1", returnedSeller.Id);
    }

    [Fact]
    public async Task Edit_Post_WithValidModel_UpdatesSeller()
    {
        // Arrange
        using var context = CreateContext();
        var seller = new Seller { Id = "1", FirstName = "John", LastName = "Doe" };
        context.Sellers.Add(seller);
        await context.SaveChangesAsync();
        context.Entry(seller).State = EntityState.Detached;

        var controller = CreateController(context);
        var updatedSeller = new Seller { Id = "1", FirstName = "Jane", LastName = "Smith" };

        // Act
        var result = await controller.Edit(updatedSeller);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        // Verify update
        var updated = await context.Sellers.FindAsync("1");
        Assert.Equal("Jane", updated!.FirstName);
    }

    [Fact]
    public async Task Delete_Get_WithValidId_ReturnsViewWithSeller()
    {
        // Arrange
        using var context = CreateContext();
        var seller = new Seller { Id = "1", FirstName = "John", LastName = "Doe" };
        context.Sellers.Add(seller);
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.Delete("1");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedSeller = Assert.IsType<Seller>(viewResult.Model);
        Assert.Equal("1", returnedSeller.Id);
    }

    [Fact]
    public async Task DeleteConfirmed_WithValidId_RemovesSeller()
    {
        // Arrange
        using var context = CreateContext();
        var seller = new Seller { Id = "1", FirstName = "John", LastName = "Doe" };
        context.Sellers.Add(seller);
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.DeleteConfirmed("1");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        // Verify deletion
        var deleted = await context.Sellers.FindAsync("1");
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Index_WithNoSellers_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var sellers = Assert.IsAssignableFrom<List<Seller>>(viewResult.Model);
        Assert.Empty(sellers);
    }
}
