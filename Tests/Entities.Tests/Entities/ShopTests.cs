using Entities;

namespace Entities.Tests.Entities;

public class ShopTests
{
    [Fact]
    public void NewShop_HasEmptyOrdersList()
    {
        var shop = new Shop();

        Assert.NotNull(shop.Orders);
        Assert.Empty(shop.Orders);
    }

    [Fact]
    public void NewShop_HasEmptySellersList()
    {
        var shop = new Shop();

        Assert.NotNull(shop.Sellers);
        Assert.Empty(shop.Sellers);
    }

    [Fact]
    public void NewShop_HasEmptyCustomersList()
    {
        var shop = new Shop();

        Assert.NotNull(shop.Customers);
        Assert.Empty(shop.Customers);
    }

    [Fact]
    public void NewShop_HasEmptyBicyclesList()
    {
        var shop = new Shop();

        Assert.NotNull(shop.Bicycles);
        Assert.Empty(shop.Bicycles);
    }

    [Fact]
    public void Shop_AddBicycle_ListUpdated()
    {
        var shop = new Shop();
        shop.Bicycles.Add(new Bicycle { TypeOfBike = "Road" });

        Assert.Single(shop.Bicycles);
    }
}
