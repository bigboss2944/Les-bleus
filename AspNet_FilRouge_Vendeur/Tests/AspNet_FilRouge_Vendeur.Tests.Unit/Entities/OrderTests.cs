using Entities;

namespace LesBleus.Tests.Unit.Entities;

public class OrderTests
{
    [Fact]
    public void NewOrder_HasEmptyBicyclesList()
    {
        var order = new Order();

        Assert.NotNull(order.Bicycles);
        Assert.Empty(order.Bicycles);
    }

    [Fact]
    public void Order_AddBicycle_ListUpdated()
    {
        var order = new Order();
        var bike = new Bicycle { TypeOfBike = "Road" };

        order.Bicycles.Add(bike);

        Assert.Single(order.Bicycles);
        Assert.Equal("Road", order.Bicycles[0].TypeOfBike);
    }

    [Fact]
    public void Order_AddMultipleBicycles_ListUpdated()
    {
        var order = new Order();
        order.Bicycles.Add(new Bicycle { TypeOfBike = "Road" });
        order.Bicycles.Add(new Bicycle { TypeOfBike = "Mountain" });
        order.Bicycles.Add(new Bicycle { TypeOfBike = "City" });

        Assert.Equal(3, order.Bicycles.Count);
    }

    [Fact]
    public void NewOrder_Discount_DefaultsToZero()
    {
        var order = new Order();

        Assert.Equal(0f, order.Discount);
    }

    [Fact]
    public void NewOrder_Tax_DefaultsToZero()
    {
        var order = new Order();

        Assert.Equal(0f, order.Tax);
    }

    [Fact]
    public void NewOrder_ShippingCost_DefaultsToZero()
    {
        var order = new Order();

        Assert.Equal(0f, order.ShippingCost);
    }

    [Fact]
    public void NewOrder_IsValidated_DefaultsToFalse()
    {
        var order = new Order();

        Assert.False(order.IsValidated);
    }

    [Fact]
    public void Order_PayMode_SetAndGet()
    {
        var order = new Order { PayMode = "Card" };

        Assert.Equal("Card", order.PayMode);
    }

    [Fact]
    public void Order_UseLoyaltyPoint_SetAndGet()
    {
        var order = new Order { UseLoyaltyPoint = true };

        Assert.True(order.UseLoyaltyPoint);
    }
}
