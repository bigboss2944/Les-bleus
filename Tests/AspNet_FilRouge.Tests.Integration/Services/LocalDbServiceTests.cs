using AspNet_FilRouge.Services;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace LesBleus.Tests.Integration.Services;

public class LocalDbServiceTests : IDisposable
{
    private readonly string _testDir;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private LocalDbService _service;

    public LocalDbServiceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid());
        Directory.CreateDirectory(_testDir);
        
        _environmentMock = new Mock<IWebHostEnvironment>();
        _environmentMock.Setup(e => e.ContentRootPath).Returns(_testDir);
        _service = new LocalDbService(_environmentMock.Object);
    }

    public void Dispose()
    {
        // Clean up test database files (WAL creates additional files)
        try
        {
            var dbPath = Path.Combine(_testDir, "local_cache.db");
            if (File.Exists(dbPath))
                File.Delete(dbPath);
            if (File.Exists($"{dbPath}-shm"))
                File.Delete($"{dbPath}-shm");
            if (File.Exists($"{dbPath}-wal"))
                File.Delete($"{dbPath}-wal");
            
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Fact]
    public async Task BulkUpsertAllAsync_WithValidData_StoresOrdersSuccessfully()
    {
        // Arrange
        var orders = new[]
        {
            new Order { IdOrder = 1, PayMode = "Card", ShippingCost = 10f, Discount = 0f, Tax = 20f },
            new Order { IdOrder = 2, PayMode = "Cash", ShippingCost = 5f, Discount = 5f, Tax = 20f }
        };

        // Act
        await _service.BulkUpsertAllAsync(orders, Array.Empty<Bicycle>(), Array.Empty<Seller>(), Array.Empty<Customer>());

        // Assert - Verify by reading from DB
        var result = await ReadOrdersFromDb();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task BulkUpsertAllAsync_WithBicycles_StoresBicyclesSuccessfully()
    {
        // Arrange
        var bicycles = new[]
        {
            new Bicycle { Id = 100, TypeOfBike = "Mountain", FreeTaxPrice = 500f },
            new Bicycle { Id = 101, TypeOfBike = "Road", FreeTaxPrice = 600f }
        };

        // Act
        await _service.BulkUpsertAllAsync(Array.Empty<Order>(), bicycles, Array.Empty<Seller>(), Array.Empty<Customer>());

        // Assert
        var result = await ReadBicyclesFromDb();
        Assert.Equal(2, result.Count);
        Assert.True(result.All(b => b.FreeTaxPrice > 0));
    }

    [Fact]
    public async Task BulkUpsertAllAsync_WithDuplicateData_UpdatesExistingRecords()
    {
        // Arrange
        var order1 = new Order { IdOrder = 1, PayMode = "Card", ShippingCost = 10f };
        await _service.BulkUpsertAllAsync(new[] { order1 }, Array.Empty<Bicycle>(), Array.Empty<Seller>(), Array.Empty<Customer>());

        // Act - Upsert with updated data
        var order1Updated = new Order { IdOrder = 1, PayMode = "Cash", ShippingCost = 15f };
        await _service.BulkUpsertAllAsync(new[] { order1Updated }, Array.Empty<Bicycle>(), Array.Empty<Seller>(), Array.Empty<Customer>());

        // Assert
        var result = await ReadOrdersFromDb();
        Assert.Single(result);
        Assert.Equal("Cash", result.First().PayMode);
        Assert.Equal(15f, result.First().ShippingCost);
    }

    [Fact]
    public async Task SaveStockRequestStatusAsync_WithValidId_UpdatesStatusSuccessfully()
    {
        // Arrange - Create and sync a stock request
        var stockRequest = new StockRequest
        {
            Id = 1,
            BicycleName = "Test Bike",
            Quantity = 5,
            RequestDate = DateTime.Now,
            Status = "Pending"
        };
        await _service.BulkUpsertStockRequestsAsync(new[] { stockRequest });

        // Act - Update the status
        await _service.SaveStockRequestStatusAsync(1, "Approved");

        // Assert - The status should be recorded in the StockRequestStatus table
        var statuses = _service.GetStockRequestStatuses();
        Assert.Contains(1, statuses.Keys);
        Assert.Equal("Approved", statuses[1]);
    }

    [Fact]
    public async Task EnqueuePendingVendorStatusUpdateAsync_WithValidData_StoresUpdate()
    {
        // Act
        await _service.EnqueuePendingVendorStatusUpdateAsync(1, "Approved", null);

        // Assert
        var pending = _service.GetPendingVendorStatusUpdates(10);
        Assert.Single(pending);
        Assert.Equal(1, pending.First().RequestId);
        Assert.Equal("Approved", pending.First().Status);
    }

    [Fact]
    public async Task DeletePendingVendorStatusUpdateAsync_WithValidId_RemovesPendingUpdate()
    {
        // Arrange
        await _service.EnqueuePendingVendorStatusUpdateAsync(1, "Approved", null);
        var pending = _service.GetPendingVendorStatusUpdates(10);
        var id = pending.First().Id;

        // Act
        await _service.DeletePendingVendorStatusUpdateAsync(id);

        // Assert
        var result = _service.GetPendingVendorStatusUpdates(10);
        Assert.Empty(result);
    }

    [Fact]
    public async Task BulkUpsertAllAsync_WithMultipleEntities_MaintainsTransactionIntegrity()
    {
        // Arrange
        var order = new Order { IdOrder = 1, PayMode = "Card" };
        var bicycle = new Bicycle { Id = 100, TypeOfBike = "Mountain", FreeTaxPrice = 500f };
        var seller = new Seller { Id = "S1", FirstName = "John", LastName = "Doe" };
        var customer = new Customer { Id = "C1", FirstName = "Jane", LastName = "Smith", PostalCode = 75000 };

        // Act
        await _service.BulkUpsertAllAsync(
            new[] { order },
            new[] { bicycle },
            new[] { seller },
            new[] { customer }
        );

        // Assert - All entities should be persisted
        var orders = await ReadOrdersFromDb();
        var bicycles = await ReadBicyclesFromDb();
        var sellers = await ReadSellersFromDb();
        var customers = await ReadCustomersFromDb();

        Assert.Single(orders);
        Assert.Single(bicycles);
        Assert.Single(sellers);
        Assert.Single(customers);
    }

    // Helper methods to read from database
    private async Task<List<Order>> ReadOrdersFromDb()
    {
        var orders = new List<Order>();
        var dbPath = Path.Combine(_testDir, "local_cache.db");
        
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Filename={dbPath}");
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT IdOrder, PayMode, ShippingCost, Discount, Tax FROM Orders";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            orders.Add(new Order
            {
                IdOrder = reader.GetInt32(0),
                PayMode = reader.IsDBNull(1) ? null : reader.GetString(1),
                ShippingCost = reader.GetFloat(2),
                Discount = reader.GetFloat(3),
                Tax = reader.GetFloat(4)
            });
        }

        return orders;
    }

    private async Task<List<Bicycle>> ReadBicyclesFromDb()
    {
        var bicycles = new List<Bicycle>();
        var dbPath = Path.Combine(_testDir, "local_cache.db");

        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Filename={dbPath}");
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, TypeOfBike, FreeTaxPrice FROM Bicycles";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            bicycles.Add(new Bicycle
            {
                Id = reader.GetInt32(0),
                TypeOfBike = reader.IsDBNull(1) ? null : reader.GetString(1),
                FreeTaxPrice = reader.GetFloat(2)
            });
        }

        return bicycles;
    }

    private async Task<List<Seller>> ReadSellersFromDb()
    {
        var sellers = new List<Seller>();
        var dbPath = Path.Combine(_testDir, "local_cache.db");

        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Filename={dbPath}");
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, FirstName, LastName FROM Sellers";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            sellers.Add(new Seller
            {
                Id = reader.GetString(0),
                FirstName = reader.IsDBNull(1) ? null : reader.GetString(1),
                LastName = reader.IsDBNull(2) ? null : reader.GetString(2)
            });
        }

        return sellers;
    }

    private async Task<List<Customer>> ReadCustomersFromDb()
    {
        var customers = new List<Customer>();
        var dbPath = Path.Combine(_testDir, "local_cache.db");

        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Filename={dbPath}");
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, FirstName, LastName, PostalCode FROM Customers";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            customers.Add(new Customer
            {
                Id = reader.GetString(0),
                FirstName = reader.IsDBNull(1) ? null : reader.GetString(1),
                LastName = reader.IsDBNull(2) ? null : reader.GetString(2),
                PostalCode = reader.GetInt32(3)
            });
        }

        return customers;
    }

    private async Task<List<StockRequest>> ReadStockRequestsFromDb()
    {
        var requests = new List<StockRequest>();
        var dbPath = Path.Combine(_testDir, "local_cache.db");

        using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Filename={dbPath}");
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, BicycleName, Quantity, RequestDate, Status FROM StockRequests";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            requests.Add(new StockRequest
            {
                Id = reader.GetInt32(0),
                BicycleName = reader.GetString(1),
                Quantity = reader.GetInt32(2),
                RequestDate = DateTime.Parse(reader.GetString(3)),
                Status = reader.GetString(4)
            });
        }

        return requests;
    }
}
