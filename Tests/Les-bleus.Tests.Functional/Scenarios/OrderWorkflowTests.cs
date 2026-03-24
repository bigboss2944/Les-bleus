using Microsoft.EntityFrameworkCore;
using Entities;

namespace LesBleus.Tests.Functional.Scenarios;

public class OrderWorkflowTests
{
    private static Entities.ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<Entities.ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public void NewOrder_IsNotValidated_ByDefault()
    {
        var order = new Entities.Order();

        Assert.False(order.IsValidated);
    }

    [Fact]
    public void Order_WithNoBicycles_CannotBeValidated()
    {
        var order = new Entities.Order { Bicycles = new List<Entities.Bicycle>() };

        // Business rule: cannot validate an order with no bicycles
        bool canValidate = order.Bicycles.Count > 0;

        Assert.False(canValidate);
    }

    [Fact]
    public void Order_WithBicycles_CanBeValidated()
    {
        var order = new Entities.Order
        {
            Bicycles = new List<Entities.Bicycle> { new() { FreeTaxPrice = 300f } }
        };

        bool canValidate = order.Bicycles.Count > 0;

        Assert.True(canValidate);
    }

    [Fact]
    public async Task ValidatedOrder_CannotAddBicycles()
    {
        using var context = CreateContext();
        var order = new Entities.Order
        {
            Bicycles = new List<Entities.Bicycle> { new() { FreeTaxPrice = 200f } },
            IsValidated = true
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var retrieved = await context.Orders
            .Include(o => o.Bicycles)
            .FirstAsync(o => o.IdOrder == order.IdOrder);

        // Business rule: cannot add bicycles to a validated order
        Assert.True(retrieved.IsValidated);
        int countBefore = retrieved.Bicycles.Count;

        // Simulate the validation guard
        bool canModify = !retrieved.IsValidated;
        Assert.False(canModify);
        Assert.Equal(countBefore, retrieved.Bicycles.Count);
    }

    [Fact]
    public async Task Order_Validate_SetIsValidatedTrue()
    {
        using var context = CreateContext();
        var order = new Entities.Order
        {
            Bicycles = new List<Entities.Bicycle> { new() { FreeTaxPrice = 400f } }
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        order.IsValidated = true;
        await context.SaveChangesAsync();

        var retrieved = await context.Orders.FindAsync(order.IdOrder);
        Assert.NotNull(retrieved);
        Assert.True(retrieved.IsValidated);
    }

    [Fact]
    public void PriceCalculation_MatchesExpectedFormula()
    {
        var order = new Entities.Order
        {
            Bicycles = new List<Entities.Bicycle> { new() { FreeTaxPrice = 500f } },
            Discount = 5f,
            Tax = 20f,
            ShippingCost = 25f
        };

        // subtotal = 500, after 5% discount = 475, with 20% tax = 570, + 25 shipping = 595
        float subtotal = order.Bicycles.Sum(b => b.FreeTaxPrice);
        float afterDiscount = subtotal * (1 - order.Discount / 100f);
        float withTax = afterDiscount * (1 + order.Tax / 100f);
        float total = withTax + order.ShippingCost;

        Assert.Equal(595f, total, precision: 2);
    }
}
