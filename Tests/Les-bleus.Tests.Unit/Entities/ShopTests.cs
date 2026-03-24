using Entities;

namespace LesBleus.Tests.Unit.Entities;

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
    public void Shop_Town_SetAndGet()
    {
        var shop = new Shop { Town = "Lyon" };

        Assert.Equal("Lyon", shop.Town);
    }

    [Fact]
    public void Shop_Nameshop_SetAndGet()
    {
        var shop = new Shop { Nameshop = "Les Bleus Cycles" };

        Assert.Equal("Les Bleus Cycles", shop.Nameshop);
    }

    [Fact]
    public void Shop_Phone_SetAndGet()
    {
        var shop = new Shop { Phone = "0456789012" };

        Assert.Equal("0456789012", shop.Phone);
    }

    [Fact]
    public void Shop_Email_SetAndGet()
    {
        var shop = new Shop { Email = "shop@lesbleus.fr" };

        Assert.Equal("shop@lesbleus.fr", shop.Email);
    }

    [Fact]
    public void Shop_Postalcode_SetAndGet()
    {
        var shop = new Shop { Postalcode = 69001 };

        Assert.Equal(69001, shop.Postalcode);
    }

    [Fact]
    public void Shop_AddBicycle_ListUpdated()
    {
        var shop = new Shop();
        shop.Bicycles.Add(new Bicycle { TypeOfBike = "Road" });

        Assert.Single(shop.Bicycles);
    }
}
