using Entities;

namespace Entities.Tests.Entities;

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
