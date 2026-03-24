using Entities;
using Microsoft.EntityFrameworkCore;

namespace LesBleus.Tests.Integration.Repositories;

public class OrderRepositoryTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task CreateOrder_CanBeSaved()
    {
        using var context = CreateContext();
        var order = new Order { PayMode = "Card", ShippingCost = 10f };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var retrieved = await context.Orders.FindAsync(order.IdOrder);

        Assert.NotNull(retrieved);
        Assert.Equal("Card", retrieved.PayMode);
    }

    [Fact]
    public async Task CreateOrderWithBicycles_RelationshipPersists()
    {
        using var context = CreateContext();
        var bike = new Bicycle { TypeOfBike = "Road", FreeTaxPrice = 500f };
        var order = new Order { Bicycles = new List<Bicycle> { bike } };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var retrieved = await context.Orders
            .Include(o => o.Bicycles)
            .FirstOrDefaultAsync(o => o.IdOrder == order.IdOrder);

        Assert.NotNull(retrieved);
        Assert.Single(retrieved.Bicycles);
        Assert.Equal("Road", retrieved.Bicycles[0].TypeOfBike);
    }

    [Fact]
    public async Task DeleteOrder_BicyclesHaveNullOrder()
    {
        using var context = CreateContext();
        var bike = new Bicycle { TypeOfBike = "Road" };
        var order = new Order { Bicycles = new List<Bicycle> { bike } };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        context.Orders.Remove(order);
        await context.SaveChangesAsync();

        var deletedOrder = await context.Orders.FindAsync(order.IdOrder);
        Assert.Null(deletedOrder);

        var orphanBike = await context.Bicycles.FindAsync(bike.Id);
        Assert.NotNull(orphanBike);
        Assert.Null(orphanBike.Order);
    }

    [Fact]
    public async Task QueryOrdersByCustomer_ReturnsCorrectOrders()
    {
        using var context = CreateContext();
        var customer = new Customer { Town = "Paris" };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        context.Orders.Add(new Order { Customer = customer });
        context.Orders.Add(new Order { Customer = customer });
        context.Orders.Add(new Order());
        await context.SaveChangesAsync();

        var customerOrders = await context.Orders
            .Where(o => o.Customer != null && o.Customer.Id == customer.Id)
            .ToListAsync();

        Assert.Equal(2, customerOrders.Count);
    }
}
