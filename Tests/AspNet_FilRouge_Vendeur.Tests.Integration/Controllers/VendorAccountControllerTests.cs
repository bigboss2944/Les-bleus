using System.Security.Claims;
using AspNet_FilRouge_Vendeur.Controllers;
using AspNet_FilRouge_Vendeur.Models;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LesBleus.Tests.Integration.Controllers;

public class VendorAccountControllerTests
{
    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        return userManager;
    }

    private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(
        Mock<UserManager<ApplicationUser>> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var schemeProvider = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
        var claimsFactory = new Mock<Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<ApplicationUser>>();
        var options = new Mock<Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Identity.IdentityOptions>>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<ApplicationUser>>>();
        var confirmation = new Mock<Microsoft.AspNetCore.Identity.IUserConfirmation<ApplicationUser>>();
        
        var signInManager = new Mock<SignInManager<ApplicationUser>>(
            userManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            options.Object,
            logger.Object,
            schemeProvider.Object,
            confirmation.Object);
        return signInManager;
    }

    private static AspNet_FilRouge_Vendeur.Controllers.AccountController CreateController(
        Mock<UserManager<ApplicationUser>> userManager,
        Mock<SignInManager<ApplicationUser>> signInManager)
    {
        var environment = new Mock<IWebHostEnvironment>();
        environment.Setup(e => e.EnvironmentName).Returns("Testing");

        var controller = new AspNet_FilRouge_Vendeur.Controllers.AccountController(userManager.Object, signInManager.Object, environment.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    [Fact]
    public void Login_Get_ReturnsViewWithNullReturnUrl()
    {
        // Arrange
        var userManager = CreateUserManagerMock();
        var signInManager = CreateSignInManagerMock(userManager);
        var controller = CreateController(userManager, signInManager);

        // Act
        var result = controller.Login(null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public void Login_Get_SetsReturnUrlInViewBag()
    {
        // Arrange
        var userManager = CreateUserManagerMock();
        var signInManager = CreateSignInManagerMock(userManager);
        var controller = CreateController(userManager, signInManager);
        var returnUrl = "/vendeur/bicycles";

        // Act
        var result = controller.Login(returnUrl);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(returnUrl, controller.ViewBag.ReturnUrl);
    }

    [Fact]
    public async Task Login_Post_WithInvalidModel_ReturnsView()
    {
        // Arrange
        var userManager = CreateUserManagerMock();
        var signInManager = CreateSignInManagerMock(userManager);
        var controller = CreateController(userManager, signInManager);
        controller.ModelState.AddModelError("Email", "Required");

        var model = new LoginViewModel { Email = "", Password = "" };

        // Act
        var result = await controller.Login(model, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Login_Post_WithNonExistentUser_ReturnsViewWithError()
    {
        // Arrange
        var userManager = CreateUserManagerMock();
        userManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser)null!);
        userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser)null!);

        var signInManager = CreateSignInManagerMock(userManager);
        var controller = CreateController(userManager, signInManager);

        var model = new LoginViewModel { Email = "nonexistent@test.com", Password = "password123" };

        // Act
        var result = await controller.Login(model, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Login_Post_WithValidCredentials_RedirectsHome()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", Email = "vendor@test.com" };
        var userManager = CreateUserManagerMock();
        userManager.Setup(u => u.FindByEmailAsync("vendor@test.com"))
            .ReturnsAsync(user);

        var signInManager = CreateSignInManagerMock(userManager);
        signInManager.Setup(s => s.PasswordSignInAsync(user, "password123", false, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var controller = CreateController(userManager, signInManager);

        var model = new LoginViewModel { Email = "vendor@test.com", Password = "password123" };

        // Act
        var result = await controller.Login(model, null);

        // Assert - RedirectToLocal with null returnUrl redirects to Home/Index
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Login_Post_WithWrongPassword_ReturnsViewWithError()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", Email = "vendor@test.com" };
        var userManager = CreateUserManagerMock();
        userManager.Setup(u => u.FindByEmailAsync("vendor@test.com"))
            .ReturnsAsync(user);

        var signInManager = CreateSignInManagerMock(userManager);
        signInManager.Setup(s => s.PasswordSignInAsync(user, "wrongpassword", false, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var controller = CreateController(userManager, signInManager);

        var model = new LoginViewModel { Email = "vendor@test.com", Password = "wrongpassword" };

        // Act
        var result = await controller.Login(model, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task ForgotPassword_Post_DoesNotExposeResetLinkOutsideDevelopment()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", Email = "vendor@test.com", UserName = "vendor@test.com" };
        var userManager = CreateUserManagerMock();
        userManager.Setup(u => u.FindByEmailAsync("vendor@test.com")).ReturnsAsync(user);
        userManager.Setup(u => u.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        userManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token");

        var signInManager = CreateSignInManagerMock(userManager);
        var environment = new Mock<IWebHostEnvironment>();
        environment.Setup(e => e.EnvironmentName).Returns("Testing");

        var controller = new AspNet_FilRouge_Vendeur.Controllers.AccountController(userManager.Object, signInManager.Object, environment.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            controller.HttpContext,
            Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
        controller.Url = Mock.Of<IUrlHelper>(u =>
            u.Action(It.IsAny<Microsoft.AspNetCore.Mvc.Routing.UrlActionContext>()) == "https://localhost/Account/ResetPassword?code=token");

        var model = new ForgotPasswordViewModel { Email = "vendor@test.com" };

        // Act
        var result = await controller.ForgotPassword(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("ForgotPasswordConfirmation", redirectResult.ActionName);
        Assert.False(controller.TempData.ContainsKey("ResetPasswordLink"));
    }
}
