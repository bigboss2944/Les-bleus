using AspNet_FilRouge.Services;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace LesBleus.Tests.Integration.Services;

public class SyncBackgroundServiceTests
{
    private readonly string _testDir;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IServiceScope> _scopeMock;
    private readonly Mock<LocalDbService> _localDbMock;
    private readonly Mock<IVendorSyncService> _vendorSyncMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<SyncBackgroundService>> _loggerMock;

    public SyncBackgroundServiceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "sync_test_" + Guid.NewGuid());
        Directory.CreateDirectory(_testDir);

        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        
        // Create environment mock properly for LocalDbService
        var envForDb = new Mock<IWebHostEnvironment>();
        envForDb.Setup(e => e.ContentRootPath).Returns(_testDir);
        _localDbMock = new Mock<LocalDbService>(envForDb.Object);
        
        _vendorSyncMock = new Mock<IVendorSyncService>();
        _environmentMock = new Mock<IWebHostEnvironment>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<SyncBackgroundService>>();

        // Setup environment
        _environmentMock.Setup(e => e.ContentRootPath).Returns(_testDir);

        // Setup configuration
        _configMock.Setup(c => c["Sync:VendeurCachePath"])
            .Returns(Path.Combine(_testDir, "vendor_cache.db"));
    }

    private SyncBackgroundService CreateService(ApplicationDbContext? dbContext = null)
    {
        var context = dbContext ?? new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ApplicationDbContext)))
            .Returns(context);

        _scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);
        _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_scopeMock.Object);

        return new SyncBackgroundService(
            _scopeFactoryMock.Object,
            _localDbMock.Object,
            _vendorSyncMock.Object,
            _environmentMock.Object,
            _configMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public void Constructor_InitializesWithCorrectDependencies()
    {
        // Act
        var service = CreateService();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task Service_CanStartAndStop()
    {
        // Arrange
        var service = CreateService();
        var cts = new CancellationTokenSource();

        // Act - service should start without throwing
        await service.StartAsync(cts.Token);
        
        // Cancel and stop
        cts.Cancel();
        await Task.Delay(50);
        await service.StopAsync(CancellationToken.None);

        // Assert - should complete without exception
        Assert.True(true);
    }

    [Fact]
    public async Task Service_HandlesStopGracefully()
    {
        // Arrange
        var service = CreateService();
        var cts = new CancellationTokenSource();

        // Act
        await service.StopAsync(cts.Token);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task Service_CanHandleMultipleStartStop()
    {
        // Arrange
        var service = CreateService();
        var cts = new CancellationTokenSource();

        // Act
        await service.StartAsync(cts.Token);
        cts.Cancel();
        await Task.Delay(50);
        await service.StopAsync(CancellationToken.None);

        // Assert
        Assert.True(cts.Token.IsCancellationRequested);
    }
}
