using AspNet_FilRouge.Controllers;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LesBleus.Tests.Integration.Controllers;

public class BicyclesControllerTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static BicyclesController CreateController(ApplicationDbContext context)
    {
        var controller = new BicyclesController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal(
                    new System.Security.Claims.ClaimsIdentity(
                        new[] 
                        { 
                            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "test"),
                            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Administrateur")
                        }, "test"))
            }
        };
        return controller;
    }

    [Fact]
    public async Task Index_ReturnsViewWithPaginatedBicycles()
    {
        // Arrange
        using var context = CreateContext();
        for (int i = 1; i <= 15; i++)
        {
            context.Bicycles.Add(new Bicycle
            {
                Id = i,
                TypeOfBike = "Mountain",
                Reference = $"REF{i:000}",
                FreeTaxPrice = 500f,
                Color = "Black"
            });
        }
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.Index(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var paginatedList = Assert.IsAssignableFrom<PaginatedList<Bicycle>>(viewResult.Model);
        Assert.True(paginatedList.Count > 0);
        Assert.NotNull(controller.ViewBag.StockSummaries);
    }

    [Fact]
    public async Task Index_Page1_ReturnsFirst10Items()
    {
        // Arrange
        using var context = CreateContext();
        for (int i = 1; i <= 25; i++)
        {
            context.Bicycles.Add(new Bicycle
            {
                Id = i,
                TypeOfBike = "Road",
                Reference = $"ROAD{i:000}",
                FreeTaxPrice = 800f,
                Color = "Red"
            });
        }
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.Index(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var paginatedList = Assert.IsAssignableFrom<PaginatedList<Bicycle>>(viewResult.Model);
        Assert.Equal(10, paginatedList.Count);
    }

    [Fact]
    public async Task Index_CalculatesStockSummaries()
    {
        // Arrange
        using var context = CreateContext();
        context.Bicycles.AddRange(
            new Bicycle { Id = 1, TypeOfBike = "Mountain", Reference = "REF001", Color = "Black", FreeTaxPrice = 500f },
            new Bicycle { Id = 2, TypeOfBike = "Mountain", Reference = "REF001", Color = "Black", FreeTaxPrice = 500f },
            new Bicycle { Id = 3, TypeOfBike = "Road", Reference = "REF002", Color = "Red", FreeTaxPrice = 800f }
        );
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.Index(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var summaries = Assert.IsAssignableFrom<List<StockSummaryViewModel>>(controller.ViewBag.StockSummaries);
        Assert.True(summaries.Count >= 2);
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
    public async Task Create_Post_WithValidModel_AddsBicycle()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);
        var bicycle = new Bicycle
        {
            TypeOfBike = "Mountain Bike",
            Reference = "MTN001",
            FreeTaxPrice = 650f,
            Color = "Blue",
            Weight = 12.5f,
            WheelSize = 29f,
            Electric = false,
            Brand = "Trek",
            Size = 17f
        };

        // Act
        var result = await controller.Create(bicycle);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        // Verify bicycle was added
        var savedBicycle = await context.Bicycles.FirstOrDefaultAsync(b => b.Reference == "MTN001");
        Assert.NotNull(savedBicycle);
        Assert.Equal(650f, savedBicycle.FreeTaxPrice);
    }

    [Fact]
    public async Task Create_Post_WithInvalidModel_ReturnsView()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);
        controller.ModelState.AddModelError("TypeOfBike", "Required");
        var bicycle = new Bicycle();

        // Act
        var result = await controller.Create(bicycle);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Create_Post_WithElectricBike_SavesCorrectly()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);
        var bicycle = new Bicycle
        {
            TypeOfBike = "E-Bike",
            Reference = "EBIKE001",
            FreeTaxPrice = 1500f,
            Color = "Silver",
            Electric = true,
            Brand = "Giant"
        };

        // Act
        await controller.Create(bicycle);

        // Assert
        var savedBike = await context.Bicycles.FirstOrDefaultAsync(b => b.Reference == "EBIKE001");
        Assert.NotNull(savedBike);
        Assert.True(savedBike.Electric);
        Assert.Equal(1500f, savedBike.FreeTaxPrice);
    }

    [Fact]
    public async Task Index_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);

        // Act
        var result = await controller.Index(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var paginatedList = Assert.IsAssignableFrom<PaginatedList<Bicycle>>(viewResult.Model);
        Assert.Empty(paginatedList);
    }

    [Fact]
    public async Task Create_Post_WithDeliverableProduct_SavesCorrectly()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);
        var bicycle = new Bicycle
        {
            TypeOfBike = "Standard Bike",
            Reference = "STD001",
            FreeTaxPrice = 400f,
            Color = "Black",
            Deliverable = true,
            Brand = "Decathlon"
        };

        // Act
        await controller.Create(bicycle);

        // Assert
        var saved = await context.Bicycles.FirstOrDefaultAsync(b => b.Reference == "STD001");
        Assert.NotNull(saved);
        Assert.True(saved.Deliverable);
    }
}
