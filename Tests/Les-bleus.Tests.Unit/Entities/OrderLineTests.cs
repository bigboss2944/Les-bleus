using AspNet_FilRouge.Models;

namespace LesBleus.Tests.Unit.Entities;

public class OrderLineTests
{
    [Fact]
    public void NewOrderLine_Quantity_DefaultsToZero()
    {
        var line = new OrderLine();

        Assert.Equal(0, line.Quantity);
    }

    [Fact]
    public void OrderLine_Quantity_SetAndGet()
    {
        var line = new OrderLine { Quantity = 3 };

        Assert.Equal(3, line.Quantity);
    }

    [Fact]
    public void OrderLine_ProductType_SetAndGet()
    {
        var pt = new ProductType { Reference = "REF-002" };
        var line = new OrderLine { ProductType = pt, ProductTypeId = 1 };

        Assert.NotNull(line.ProductType);
        Assert.Equal("REF-002", line.ProductType.Reference);
    }

    [Fact]
    public void Order_NewOrder_HasEmptyOrderLines()
    {
        var order = new Order();

        Assert.NotNull(order.OrderLines);
        Assert.Empty(order.OrderLines);
    }

    [Fact]
    public void Order_AddOrderLine_ListUpdated()
    {
        var order = new Order();
        var pt = new ProductType { Reference = "REF-003" };
        var line = new OrderLine { ProductType = pt, Quantity = 2 };

        order.OrderLines.Add(line);

        Assert.Single(order.OrderLines);
        Assert.Equal(2, order.OrderLines[0].Quantity);
    }

    [Fact]
    public void Order_Discount_IsPercentage()
    {
        var order = new Order { Discount = 10f };

        Assert.Equal(10f, order.Discount);
    }
}
