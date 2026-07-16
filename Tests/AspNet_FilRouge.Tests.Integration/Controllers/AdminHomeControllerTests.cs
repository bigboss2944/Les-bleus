using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Controllers;

namespace LesBleus.Tests.Integration.Controllers;

// HomeController is shared between the Admin and Vendeur apps (Shared/Controllers/HomeController.cs);
// this single suite covers it for both, so no per-app duplicate exists.
public class HomeControllerTests
{
    private static HomeController CreateController()
    {
        var controller = new HomeController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    [Fact]
    public void Index_ReturnsView()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public void About_ReturnsViewWithMessage()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = controller.About();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(controller.ViewBag.Message);
        Assert.Contains("story", controller.ViewBag.Message.ToString()!);
    }

    [Fact]
    public void About_SetCorrectMessage()
    {
        // Arrange
        var controller = CreateController();

        // Act
        controller.About();

        // Assert
        Assert.Equal("Les bleus, our story.", controller.ViewBag.Message);
    }

    [Fact]
    public void Contact_ReturnsViewWithMessage()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = controller.Contact();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(controller.ViewBag.Message);
    }

    [Fact]
    public void Contact_SetCorrectMessage()
    {
        // Arrange
        var controller = CreateController();

        // Act
        controller.Contact();

        // Assert
        Assert.Equal("Please, for any question.", controller.ViewBag.Message);
    }
}
