using Entities;

namespace Entities.Tests.Entities;

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
    public void Customer_AddOrder_ListUpdated()
    {
        var customer = new Customer();
        customer.Orders.Add(new Order());

        Assert.Single(customer.Orders);
    }
}
