using Entities;
using Microsoft.EntityFrameworkCore;

namespace Entities.Tests.Repositories;

public class CustomerRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetCustomersWithShopAndOrderCountAsync_ReturnsShopNameAndOrderCount_PerCustomer()
    {
        using var db = CreateContext();

        var shop = new Shop { Name = "Boutique Paris" };
        var customerWithOrders = new Customer
        {
            Id = "cust-1",
            LastName = "Dupont",
            Shop = shop,
        };
        var customerWithoutOrders = new Customer
        {
            Id = "cust-2",
            LastName = "Martin",
            Shop = shop,
        };

        db.Shops.Add(shop);
        db.Customers.AddRange(customerWithOrders, customerWithoutOrders);
        db.Orders.AddRange(
            new Order { Customer = customerWithOrders, Date = DateTime.Today },
            new Order { Customer = customerWithOrders, Date = DateTime.Today.AddDays(-1) }
        );
        await db.SaveChangesAsync();

        var repository = new CustomerRepository(db);

        var result = await repository.GetCustomersWithShopAndOrderCountAsync();

        var dupont = Assert.Single(result, r => r.CustomerId == "cust-1");
        Assert.Equal("Boutique Paris", dupont.ShopName);
        Assert.Equal(2, dupont.OrderCount);

        var martin = Assert.Single(result, r => r.CustomerId == "cust-2");
        Assert.Equal("Boutique Paris", martin.ShopName);
        Assert.Equal(0, martin.OrderCount);
    }
}
