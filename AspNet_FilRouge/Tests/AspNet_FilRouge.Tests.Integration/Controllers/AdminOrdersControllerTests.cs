using System.Security.Claims;
using AspNet_FilRouge.Controllers;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LesBleus.Tests.Integration.Controllers;

public class AdminOrdersControllerTests
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
        userManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user-id");
        return userManager;
    }

    private static OrdersController CreateController(ApplicationDbContext context)
    {
        var userManager = CreateUserManagerMock();
        var controller = new OrdersController(context, userManager.Object);
        
        // Create a ClaimsPrincipal with both Name and Admin role
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "test"),
            new Claim(ClaimTypes.NameIdentifier, "user-id"),
            new Claim(ClaimTypes.Role, "Administrateur")
        };
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"))
            }
        };
        return controller;
    }

    [Fact]
    public async Task Index_ReturnsViewWithPaginatedList()
    {
        using var context = CreateContext();
        context.Orders.AddRange(new Order { PayMode = "Card" }, new Order { PayMode = "Cash" });
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<PaginatedList<Order>>(viewResult.Model);
    }

    [Fact]
    public async Task Details_NullId_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Details(null);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Details_NonExistentId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Details(9999L);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ExistingId_ReturnsViewWithOrder()
    {
        using var context = CreateContext();
        var order = new Order { PayMode = "Card", ShippingCost = 15f };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.Details(order.IdOrder);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Order>(viewResult.Model);
        Assert.Equal("Card", model.PayMode);
    }

    [Fact]
    public async Task GetPrice_ReturnsCorrectTotal()
    {
        using var context = CreateContext();
        var order = new Order
        {
            Bicycles = new List<Bicycle> { new() { FreeTaxPrice = 200f } },
            Discount = 10f,
            Tax = 20f,
            ShippingCost = 5f
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.GetPrice(order.IdOrder);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        // subtotal=200, after 10% discount=180, with 20% tax=216, + shipping 5 = 221
        var totalProp = okResult.Value!.GetType().GetProperty("total");
        Assert.NotNull(totalProp);
        var total = (float)totalProp!.GetValue(okResult.Value)!;
        Assert.Equal(221f, total, precision: 2);
    }
}
