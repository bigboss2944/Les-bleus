using System.Security.Claims;
using AspNet_FilRouge_Vendeur.Controllers;
using AspNet_FilRouge_Vendeur.Models;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LesBleus.Tests.Integration.Controllers;

public class VendorOrdersControllerTests
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

    private static OrdersController CreateController(
        ApplicationDbContext context,
        Mock<UserManager<ApplicationUser>> userManager,
        bool isAdmin,
        string userId = "admin-1")
    {
        userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, "test")
        };

        if (isAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Administrateur"));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "Vendeur"));
        }

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"))
        };

        var controller = new OrdersController(context, userManager.Object, new OrderPricingService())
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };

        return controller;
    }

    [Fact]
    public async Task Validate_WhenClientOffline_ReturnsBadRequestAndDoesNotValidate()
    {
        using var context = CreateContext();
        var userManager = CreateUserManagerMock();
        var controller = CreateController(context, userManager, isAdmin: true);

        var order = new Order { Date = DateTime.UtcNow, IsValidated = false };
        var bicycle = new Bicycle { Quantity = 2, FreeTaxPrice = 100f, Order = order };
        context.Orders.Add(order);
        context.Bicycles.Add(bicycle);
        await context.SaveChangesAsync();

        controller.Request.Headers["X-Client-Online"] = "false";

        var result = await controller.Validate(order.IdOrder);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("hors-ligne", badRequest.Value?.ToString(), StringComparison.OrdinalIgnoreCase);

        var persisted = await context.Orders.FirstAsync(o => o.IdOrder == order.IdOrder);
        Assert.False(persisted.IsValidated);
    }

    [Fact]
    public async Task Validate_WhenClientOnline_ValidatesOrderAndDecrementsStock()
    {
        using var context = CreateContext();
        var userManager = CreateUserManagerMock();
        var controller = CreateController(context, userManager, isAdmin: true);

        var order = new Order { Date = DateTime.UtcNow, IsValidated = false };
        var bicycle = new Bicycle { Quantity = 2, FreeTaxPrice = 100f, Order = order };
        context.Orders.Add(order);
        context.Bicycles.Add(bicycle);
        await context.SaveChangesAsync();

        controller.Request.Headers["X-Client-Online"] = "true";

        var result = await controller.Validate(order.IdOrder);

        Assert.IsType<OkObjectResult>(result);

        var persistedOrder = await context.Orders.FirstAsync(o => o.IdOrder == order.IdOrder);
        var persistedBike = await context.Bicycles.FirstAsync(b => b.Id == bicycle.Id);
        Assert.True(persistedOrder.IsValidated);
        Assert.Equal(1, persistedBike.Quantity);
    }
}
