using System.Security.Claims;
using AspNet_FilRouge.Controllers;
using AspNet_FilRouge.Services;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LesBleus.Tests.Integration.Controllers;

public class AdminStockRequestsControllerTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static StockRequestsController CreateController(
        ApplicationDbContext context,
        Mock<IVendorSyncService>? vendorSyncMock = null)
    {
        var userManager = CreateUserManagerMock();

        vendorSyncMock ??= new Mock<IVendorSyncService>();
        vendorSyncMock
            .Setup(s => s.WriteStatusToVendorCacheAsync(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var controller = new StockRequestsController(context, userManager.Object, vendorSyncMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.Name, "admin") }, "test"))
            }
        };
        return controller;
    }

    [Fact]
    public async Task Approve_ExistingRequest_UpdatesStatusToApprouvee()
    {
        using var context = CreateContext();
        var request = new StockRequest { BicycleName = "Trek FX3", Quantity = 2, Status = "En attente" };
        context.StockRequests.Add(request);
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.Approve(request.Id);

        Assert.IsType<RedirectToActionResult>(result);
        var updated = await context.StockRequests.FindAsync(request.Id);
        Assert.Equal("Approuvée", updated!.Status);
    }

    [Fact]
    public async Task Approve_ExistingRequest_WritesStatusToVendorCache()
    {
        using var context = CreateContext();
        var request = new StockRequest { BicycleName = "Trek FX3", Quantity = 2, Status = "En attente" };
        context.StockRequests.Add(request);
        await context.SaveChangesAsync();

        var vendorSyncMock = new Mock<IVendorSyncService>();
        vendorSyncMock
            .Setup(s => s.WriteStatusToVendorCacheAsync(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        var controller = CreateController(context, vendorSyncMock);

        await controller.Approve(request.Id);

        vendorSyncMock.Verify(
            s => s.WriteStatusToVendorCacheAsync(request.Id, "Approuvée"),
            Times.Once);
    }

    [Fact]
    public async Task Reject_ExistingRequest_UpdatesStatusToRejetee()
    {
        using var context = CreateContext();
        var request = new StockRequest { BicycleName = "Specialized Allez", Quantity = 1, Status = "En attente" };
        context.StockRequests.Add(request);
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.Reject(request.Id);

        Assert.IsType<RedirectToActionResult>(result);
        var updated = await context.StockRequests.FindAsync(request.Id);
        Assert.Equal("Rejetée", updated!.Status);
    }

    [Fact]
    public async Task Reject_ExistingRequest_WritesStatusToVendorCache()
    {
        using var context = CreateContext();
        var request = new StockRequest { BicycleName = "Specialized Allez", Quantity = 1, Status = "En attente" };
        context.StockRequests.Add(request);
        await context.SaveChangesAsync();

        var vendorSyncMock = new Mock<IVendorSyncService>();
        vendorSyncMock
            .Setup(s => s.WriteStatusToVendorCacheAsync(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        var controller = CreateController(context, vendorSyncMock);

        await controller.Reject(request.Id);

        vendorSyncMock.Verify(
            s => s.WriteStatusToVendorCacheAsync(request.Id, "Rejetée"),
            Times.Once);
    }

    [Fact]
    public async Task Approve_NonExistentId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Approve(9999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Reject_NonExistentId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Reject(9999);

        Assert.IsType<NotFoundResult>(result);
    }
}
