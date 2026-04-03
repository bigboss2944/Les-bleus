using AspNet_FilRouge_Vendeur.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LesBleus.Tests.Integration.Controllers;

public class VendorHomeControllerTests
{
    private static AspNet_FilRouge_Vendeur.Controllers.HomeController CreateController()
    {
        var controller = new AspNet_FilRouge_Vendeur.Controllers.HomeController();
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
    public void About_SetsCorrectMessage()
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
    public void Contact_SetsCorrectMessage()
    {
        // Arrange
        var controller = CreateController();

        // Act
        controller.Contact();

        // Assert
        Assert.Equal("Please, for any question.", controller.ViewBag.Message);
    }
}
