using AspNet_FilRouge.Controllers;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LesBleus.Tests.Integration.Controllers;

public class CustomersControllerTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static CustomersController CreateController(ApplicationDbContext context)
    {
        var controller = new CustomersController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal(
                    new System.Security.Claims.ClaimsIdentity(
                        new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "test") }, "test"))
            }
        };
        return controller;
    }

    [Fact]
    public async Task Index_ReturnsViewWithAllCustomers()
    {
        // Arrange
        using var context = CreateContext();
        var customers = new[] 
        {
            new Customer { Id = "1", FirstName = "John", LastName = "Doe", Email = "john@test.com" },
            new Customer { Id = "2", FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" }
        };
        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedCustomers = Assert.IsAssignableFrom<List<Customer>>(viewResult.Model);
        Assert.Equal(2, returnedCustomers.Count);
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithCustomer()
    {
        // Arrange
        using var context = CreateContext();
        var customer = new Customer { Id = "1", FirstName = "John", LastName = "Doe" };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        // Act
        var result = await controller.Details("1");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedCustomer = Assert.IsType<Customer>(viewResult.Model);
        Assert.Equal("1", returnedCustomer.Id);
        Assert.Equal("John", returnedCustomer.FirstName);
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
    public async Task Create_Post_WithValidModel_AddsCustomer()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);
        var customer = new Customer
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Phone = "0123456789",
            Address = "123 Main St",
            PostalCode = 75001,
            Town = "Paris"
        };

        // Act
        var result = await controller.Create(customer);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        
        // Verify customer was added to database
        var savedCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Email == "john@test.com");
        Assert.NotNull(savedCustomer);
        Assert.Equal("John", savedCustomer.FirstName);
    }

    [Fact]
    public async Task Create_Post_WithInvalidModel_ReturnsView()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);
        controller.ModelState.AddModelError("Email", "Required");
        var customer = new Customer { FirstName = "John" };

        // Act
        var result = await controller.Create(customer);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Create_Post_WithLoyaltyPoints_SavesCorrectly()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);
        var customer = new Customer
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            LoyaltyPoints = 100
        };

        // Act
        await controller.Create(customer);

        // Assert
        var savedCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Email == "jane@test.com");
        Assert.NotNull(savedCustomer);
        Assert.Equal(100, savedCustomer.LoyaltyPoints);
    }

    [Fact]
    public async Task Index_WithNoCustomers_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var controller = CreateController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var customers = Assert.IsAssignableFrom<List<Customer>>(viewResult.Model);
        Assert.Empty(customers);
    }
}
