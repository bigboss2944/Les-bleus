using Entities;
using Microsoft.EntityFrameworkCore;

namespace Entities.Tests.Repositories;

public class SellerRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetOrdersBySellerIdExplicitJoinAsync_ReturnsSameOrdersAs_NavigationVersion()
    {
        using var db = CreateContext();

        var seller = new Seller { Id = "seller-1", LastName = "Bernard" };
        var otherSeller = new Seller { Id = "seller-2", LastName = "Petit" };

        db.Sellers.AddRange(seller, otherSeller);
        db.Orders.AddRange(
            new Order { Seller = seller, Date = DateTime.Today },
            new Order { Seller = seller, Date = DateTime.Today.AddDays(-2) },
            new Order { Seller = otherSeller, Date = DateTime.Today }
        );
        await db.SaveChangesAsync();

        var repository = new SellerRepository(db);

        var expected = await repository.GetOrdersBySellerIdAsync("seller-1");
        var actual = await repository.GetOrdersBySellerIdExplicitJoinAsync("seller-1");

        Assert.Equal(
            expected.Select(o => o.IdOrder).OrderBy(id => id),
            actual.Select(o => o.IdOrder).OrderBy(id => id));
        Assert.All(actual, o => Assert.Equal("seller-1", o.Seller!.Id));
    }
}
