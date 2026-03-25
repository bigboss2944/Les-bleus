using Entities;

namespace LesBleus.Tests.Unit.Entities;

public class BicycleTests
{
    [Fact]
    public void NewBicycle_HasFalseDefaults()
    {
        var bike = new Bicycle();

        Assert.False(bike.Exchangeable);
        Assert.False(bike.Insurance);
        Assert.False(bike.Deliverable);
    }

    [Fact]
    public void Bicycle_TypeOfBike_SetAndGet()
    {
        var bike = new Bicycle { TypeOfBike = "Mountain" };

        Assert.Equal("Mountain", bike.TypeOfBike);
    }

    [Fact]
    public void Bicycle_Category_SetAndGet()
    {
        var bike = new Bicycle { Category = "Sport" };

        Assert.Equal("Sport", bike.Category);
    }

    [Fact]
    public void Bicycle_FreeTaxPrice_SetAndGet()
    {
        var bike = new Bicycle { FreeTaxPrice = 599.99f };

        Assert.Equal(599.99f, bike.FreeTaxPrice);
    }

    [Fact]
    public void Bicycle_Reference_SetAndGet()
    {
        var bike = new Bicycle { Reference = "REF-001" };

        Assert.Equal("REF-001", bike.Reference);
    }

    [Fact]
    public void Bicycle_BoolProperties_SetAndGet()
    {
        var bike = new Bicycle
        {
            Exchangeable = true,
            Insurance = true,
            Deliverable = true
        };

        Assert.True(bike.Exchangeable);
        Assert.True(bike.Insurance);
        Assert.True(bike.Deliverable);
    }

    [Fact]
    public void Bicycle_ToString_ContainsId()
    {
        var bike = new Bicycle { Id = 42, TypeOfBike = "Road", FreeTaxPrice = 300f };
        var result = bike.ToString();

        Assert.Contains("42", result);
    }

    [Fact]
    public void Bicycle_ToString_ContainsTypeOfBike()
    {
        var bike = new Bicycle { Id = 1, TypeOfBike = "Road", FreeTaxPrice = 300f };
        var result = bike.ToString();

        Assert.Contains("Road", result);
    }

    [Fact]
    public void Bicycle_ToString_ContainsFreeTaxPrice()
    {
        var bike = new Bicycle { Id = 1, TypeOfBike = "Road", FreeTaxPrice = 300f };
        var result = bike.ToString();

        Assert.Contains("300", result);
    }
}
