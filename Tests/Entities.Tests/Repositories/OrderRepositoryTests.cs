using Entities;
using Microsoft.EntityFrameworkCore;

namespace Entities.Tests.Repositories;

public class OrderRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetOrderLinesWithProductTypeAsync_ReturnsOnlyLinesForGivenOrder()
    {
        using var db = CreateContext();

        var targetOrder = new Order { Date = DateTime.Today };
        var otherOrder = new Order { Date = DateTime.Today };
        var productType = new ProductType { Reference = "ACC-CASQUE" };

        db.Orders.AddRange(targetOrder, otherOrder);
        db.ProductTypes.Add(productType);
        db.OrderLines.AddRange(
            new OrderLine { Order = targetOrder, ProductType = productType, Quantity = 3 },
            new OrderLine { Order = otherOrder, ProductType = productType, Quantity = 5 }
        );
        await db.SaveChangesAsync();

        var repository = new OrderRepository(db);

        var result = await repository.GetOrderLinesWithProductTypeAsync(targetOrder.IdOrder);

        var line = Assert.Single(result);
        Assert.Equal("ACC-CASQUE", line.ProductTypeReference);
        Assert.Equal(3, line.Quantity);
    }
}
