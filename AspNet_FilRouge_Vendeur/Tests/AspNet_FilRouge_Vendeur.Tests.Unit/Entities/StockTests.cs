using Entities;

namespace LesBleus.Tests.Unit.Entities;

public class StockTests
{
    [Fact]
    public void NewStock_Quantity_DefaultsToZero()
    {
        var stock = new Stock();

        Assert.Equal(0, stock.Quantity);
    }

    [Fact]
    public void Stock_Quantity_SetAndGet()
    {
        var stock = new Stock { Quantity = 15 };

        Assert.Equal(15, stock.Quantity);
    }

    [Fact]
    public void Stock_ProductType_SetAndGet()
    {
        var pt = new ProductType { Reference = "REF-001" };
        var stock = new Stock { ProductType = pt, ProductTypeId = 1 };

        Assert.NotNull(stock.ProductType);
        Assert.Equal("REF-001", stock.ProductType.Reference);
    }

    [Fact]
    public void Stock_ToString_ContainsQuantity()
    {
        var stock = new Stock { Id = 1, ProductTypeId = 2, Quantity = 10 };
        var result = stock.ToString();

        Assert.Contains("10", result);
    }
}
