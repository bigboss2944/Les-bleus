using Entities;

namespace Entities.Tests.Entities;

public class StockTests
{
    [Fact]
    public void NewStock_Quantity_DefaultsToZero()
    {
        var stock = new Stock();

        Assert.Equal(0, stock.Quantity);
    }

    [Fact]
    public void Stock_ToString_ContainsQuantity()
    {
        var stock = new Stock { Id = 1, ProductTypeId = 2, Quantity = 10 };
        var result = stock.ToString();

        Assert.Contains("10", result);
    }
}
