using AspNet_FilRouge.Services;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace LesBleus.Tests.Integration.Services;

public class VendorSyncServiceTests : IDisposable
{
    private readonly string _testAdminDir;
    private readonly string _testVendorDir;
    private readonly string _testAdminDbPath;
    private readonly string _testVendorDbPath;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<VendorSyncService>> _loggerMock;
    private readonly LocalDbService _localDbService;
    private readonly VendorSyncService _vendorSyncService;

    public VendorSyncServiceTests()
    {
        _testAdminDir = Path.Combine(Path.GetTempPath(), "admin_test_" + Guid.NewGuid());
        _testVendorDir = Path.Combine(Path.GetTempPath(), "vendor_test_" + Guid.NewGuid());
        Directory.CreateDirectory(_testAdminDir);
        Directory.CreateDirectory(_testVendorDir);

        _testAdminDbPath = Path.Combine(_testAdminDir, "local_cache.db");
        _testVendorDbPath = Path.Combine(_testVendorDir, "local_cache.db");

        // Setup environment mock
        _environmentMock = new Mock<IWebHostEnvironment>();
        _environmentMock.Setup(e => e.ContentRootPath).Returns(_testAdminDir);

        // Setup configuration mock
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["Sync:VendeurCachePath"])
            .Returns(_testVendorDbPath);

        // Setup logger mock
        _loggerMock = new Mock<ILogger<VendorSyncService>>();

        // Create services
        _localDbService = new LocalDbService(_environmentMock.Object);
        _vendorSyncService = new VendorSyncService(
            _environmentMock.Object,
            _configMock.Object,
            _localDbService,
            _loggerMock.Object
        );

        // Initialize vendor database
        InitializeVendorDatabase();
    }

    private void InitializeVendorDatabase()
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Filename={_testVendorDbPath}");
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS StockRequests (
                Id INTEGER PRIMARY KEY,
                BicycleName TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                RequestDate TEXT NOT NULL,
                Status TEXT NOT NULL,
                RequestedById TEXT NULL,
                Notes TEXT NULL,
                SyncedAt TEXT NULL
            )";
        cmd.ExecuteNonQuery();

        // Insert test data
        cmd.CommandText = @"
            INSERT INTO StockRequests (Id, BicycleName, Quantity, RequestDate, Status)
            VALUES (1, 'Mountain Bike', 5, '2026-03-25', 'Pending')";
        cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
        CleanupDirectory(_testAdminDir);
        CleanupDirectory(_testVendorDir);
    }

    private void CleanupDirectory(string dirPath)
    {
        try
        {
            if (Directory.Exists(dirPath))
                Directory.Delete(dirPath, true);
        }
        catch { /* Ignore cleanup errors */ }
    }

    private void CleanupDatabase(string dbPath)
    {
        try
        {
            if (File.Exists(dbPath))
                File.Delete(dbPath);
            if (File.Exists($"{dbPath}-shm"))
                File.Delete($"{dbPath}-shm");
            if (File.Exists($"{dbPath}-wal"))
                File.Delete($"{dbPath}-wal");
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Fact]
    public async Task WriteStatusToVendorCacheAsync_WithValidData_UpdatesStatusInVendorDb()
    {
        // Act
        await _vendorSyncService.WriteStatusToVendorCacheAsync(1, "Approved");

        // Assert - Verify status was updated in vendor DB
        var status = await ReadStockRequestStatusFromVendorDb(1);
        Assert.Equal("Approved", status);
    }

    [Fact]
    public async Task WriteStatusToVendorCacheAsync_WhenVendorDbNotExists_EnqueuesPendingUpdate()
    {
        // Arrange - Delete vendor database to simulate offline scenario
        CleanupDatabase(_testVendorDbPath);
        var newConfigMock = new Mock<IConfiguration>();
        newConfigMock.Setup(c => c["Sync:VendeurCachePath"])
            .Returns(_testVendorDbPath);

        var vendorSyncWithMissingDb = new VendorSyncService(
            _environmentMock.Object,
            newConfigMock.Object,
            _localDbService,
            _loggerMock.Object
        );

        // Act
        await vendorSyncWithMissingDb.WriteStatusToVendorCacheAsync(1, "Rejected");

        // Assert - Verify update was queued
        var pending = _localDbService.GetPendingVendorStatusUpdates(10);
        Assert.NotEmpty(pending);
        Assert.Single(pending, p => p.RequestId == 1 && p.Status == "Rejected");
    }

    [Fact]
    public async Task FlushPendingUpdatesAsync_WithPendingUpdates_AppliesThem()
    {
        // Arrange - Queue some updates
        await _localDbService.EnqueuePendingVendorStatusUpdateAsync(1, "Approved", null);

        // Act
        var flushed = await _vendorSyncService.FlushPendingUpdatesAsync();

        // Assert
        Assert.True(flushed > 0);
        var status = await ReadStockRequestStatusFromVendorDb(1);
        Assert.Equal("Approved", status);
    }

    [Fact]
    public async Task FlushPendingUpdatesAsync_WithNoPendingUpdates_ReturnsZero()
    {
        // Arrange - Ensure no pending updates
        var pending = _localDbService.GetPendingVendorStatusUpdates(10);
        foreach (var item in pending)
        {
            await _localDbService.DeletePendingVendorStatusUpdateAsync(item.Id);
        }

        // Act
        var flushed = await _vendorSyncService.FlushPendingUpdatesAsync();

        // Assert
        Assert.Equal(0, flushed);
    }

    [Fact]
    public async Task FlushPendingUpdatesAsync_RemovesPendingUpdatesAfterSuccess()
    {
        // Arrange
        await _localDbService.EnqueuePendingVendorStatusUpdateAsync(1, "Approved", null);
        var beforeFlush = _localDbService.GetPendingVendorStatusUpdates(10);
        Assert.NotEmpty(beforeFlush);

        // Act
        await _vendorSyncService.FlushPendingUpdatesAsync();

        // Assert
        var afterFlush = _localDbService.GetPendingVendorStatusUpdates(10);
        Assert.Empty(afterFlush);
    }

    [Fact]
    public async Task WriteStatusToVendorCacheAsync_WithMultipleUpdates_AllProcessedCorrectly()
    {
        // Act
        await _vendorSyncService.WriteStatusToVendorCacheAsync(1, "Approved");

        // Assert
        var status = await ReadStockRequestStatusFromVendorDb(1);
        Assert.Equal("Approved", status);
    }

    private async Task<string> ReadStockRequestStatusFromVendorDb(int requestId)
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Filename={_testVendorDbPath}");
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Status FROM StockRequests WHERE Id = @Id";
        cmd.Parameters.AddWithValue("@Id", requestId);

        var result = await cmd.ExecuteScalarAsync();
        return result?.ToString() ?? string.Empty;
    }
}
