using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendeurControllers = AspNet_FilRouge_Vendeur.Controllers;
using Entities;

namespace LesBleus.Tests.Integration.Controllers;

public class VendeurBicyclesControllerTests
{
    private static Entities.ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<Entities.ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static VendeurControllers.BicyclesController CreateController(Entities.ApplicationDbContext context)
    {
        var controller = new VendeurControllers.BicyclesController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.Name, "test") }, "test"))
            }
        };
        return controller;
    }

    [Fact]
    public async Task Index_ReturnsView()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Index();

        Assert.IsType<ViewResult>(result);
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
    public async Task Details_ExistingId_ReturnsViewWithBicycle()
    {
        using var context = CreateContext();
        var bike = new Entities.Bicycle { TypeOfBike = "City", FreeTaxPrice = 350f };
        context.Bicycles.Add(bike);
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.Details(bike.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Entities.Bicycle>(viewResult.Model);
        Assert.Equal("City", model.TypeOfBike);
    }

    [Fact]
    public async Task Create_ValidModel_AddsBicycleAndRedirectsToIndex()
    {
        using var context = CreateContext();
        var controller = CreateController(context);
        var bike = new Entities.Bicycle
        {
            TypeOfBike = "Road",
            Category = "Sport",
            FreeTaxPrice = 600f,
            Exchangeable = false,
            Insurance = true,
            Deliverable = true
        };

        var result = await controller.Create(bike);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal(1, await context.Bicycles.CountAsync());
    }
}
