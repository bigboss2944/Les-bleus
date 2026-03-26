using System.Security.Claims;
using AspNet_FilRouge.Controllers;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LesBleus.Tests.Integration.Controllers;

public class AdminOrdersControllerTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static OrdersController CreateController(ApplicationDbContext context, bool isAdmin = true)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "test"),
            new(ClaimTypes.NameIdentifier, "user-id"),
            new(ClaimTypes.Role, isAdmin ? "Administrateur" : "Vendeur")
        };

        var controller = new OrdersController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"))
                }
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
    public async Task Index_AsVendeur_ReturnsAllOrders()
    {
        using var context = CreateContext();
        var seller1 = new Seller { Id = "seller-1", UserName = "s1@test.com", Email = "s1@test.com" };
        var seller2 = new Seller { Id = "seller-2", UserName = "s2@test.com", Email = "s2@test.com" };
        context.Sellers.AddRange(seller1, seller2);
        context.Orders.AddRange(
            new Order { PayMode = "Card", Seller = seller1 },
            new Order { PayMode = "Cash", Seller = seller2 });
        await context.SaveChangesAsync();

        var controller = CreateController(context, isAdmin: false);
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var list = Assert.IsAssignableFrom<PaginatedList<Order>>(viewResult.Model);
        Assert.Equal(2, list.Count);
        Assert.Contains(list, o => o.Seller?.Id == "seller-1");
        Assert.Contains(list, o => o.Seller?.Id == "seller-2");
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
    public async Task Details_AsVendeur_CanViewAnyOrder()
    {
        using var context = CreateContext();
        var otherSeller = new Seller { Id = "other-seller", UserName = "other@test.com", Email = "other@test.com" };
        context.Sellers.Add(otherSeller);
        var order = new Order { PayMode = "Virement", Seller = otherSeller };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var controller = CreateController(context, isAdmin: false);
        var result = await controller.Details(order.IdOrder);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Order>(viewResult.Model);
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
