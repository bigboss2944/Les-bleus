using Entities;
using Microsoft.EntityFrameworkCore;

namespace Entities.Tests.Repositories;

public class BicycleRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetSoldBicyclesWithOrderDateAsync_ReturnsOnlyBicyclesWithAnOrder()
    {
        using var db = CreateContext();

        var order = new Order { Date = new DateTime(2026, 1, 15) };
        var soldBicycle = new Bicycle { Reference = "VTT-001", Order = order };
        var unsoldBicycle = new Bicycle { Reference = "VTT-002" };

        db.Orders.Add(order);
        db.Bicycles.AddRange(soldBicycle, unsoldBicycle);
        await db.SaveChangesAsync();

        var repository = new BicycleRepository(db);

        var result = await repository.GetSoldBicyclesWithOrderDateAsync();

        var sold = Assert.Single(result);
        Assert.Equal("VTT-001", sold.Reference);
        Assert.Equal(new DateTime(2026, 1, 15), sold.OrderDate);
    }
}
