using Entities;

namespace LesBleus.Tests.Unit.Entities;

public class CustomerTests
{
    [Fact]
    public void NewCustomer_LoyaltyPoints_DefaultsToZero()
    {
        var customer = new Customer();

        Assert.Equal(0, customer.LoyaltyPoints);
    }

    [Fact]
    public void NewCustomer_HasEmptyOrdersList()
    {
        var customer = new Customer();

        Assert.NotNull(customer.Orders);
        Assert.Empty(customer.Orders);
    }

    [Fact]
    public void Customer_Town_SetAndGet()
    {
        var customer = new Customer { Town = "Paris" };

        Assert.Equal("Paris", customer.Town);
    }

    [Fact]
    public void Customer_PostalCode_SetAndGet()
    {
        var customer = new Customer { PostalCode = 75001 };

        Assert.Equal(75001, customer.PostalCode);
    }

    [Fact]
    public void Customer_Address_SetAndGet()
    {
        var customer = new Customer { Address = "1 Rue de la Paix" };

        Assert.Equal("1 Rue de la Paix", customer.Address);
    }

    [Fact]
    public void Customer_LoyaltyPoints_SetAndGet()
    {
        var customer = new Customer { LoyaltyPoints = 150 };

        Assert.Equal(150, customer.LoyaltyPoints);
    }

    [Fact]
    public void Customer_Phone_SetAndGet()
    {
        var customer = new Customer { Phone = "0612345678" };

        Assert.Equal("0612345678", customer.Phone);
    }

    [Fact]
    public void Customer_Email_SetAndGet()
    {
        var customer = new Customer { Email = "test@example.com" };

        Assert.Equal("test@example.com", customer.Email);
    }

    [Fact]
    public void Customer_AddOrder_ListUpdated()
    {
        var customer = new Customer();
        customer.Orders.Add(new Order());

        Assert.Single(customer.Orders);
    }
}
