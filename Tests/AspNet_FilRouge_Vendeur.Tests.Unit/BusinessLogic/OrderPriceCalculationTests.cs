using Entities;

namespace LesBleus.Tests.Unit.BusinessLogic;

public class OrderPriceCalculationTests
{
    private static float CalculateTotal(Order order)
    {
        float subtotal = order.Bicycles?.Sum(b => b.FreeTaxPrice) ?? 0f;
        float afterDiscount = subtotal * (1 - order.Discount / 100f);
        float withTax = afterDiscount * (1 + order.Tax / 100f);
        return withTax + order.ShippingCost;
    }

    [Fact]
    public void EmptyOrder_TotalEqualsShippingCostOnly()
    {
        var order = new Order { ShippingCost = 15f };

        var total = CalculateTotal(order);

        Assert.Equal(15f, total);
    }

    [Fact]
    public void SingleBicycle_NoDiscountNoTax_TotalIsPricePlusShipping()
    {
        var order = new Order
        {
            Bicycles = new List<Bicycle> { new() { FreeTaxPrice = 500f } },
            Discount = 0f,
            Tax = 0f,
            ShippingCost = 20f
        };

        var total = CalculateTotal(order);

        Assert.Equal(520f, total);
    }

    [Fact]
    public void Bicycle_WithTenPercentDiscount_SubtotalReduced()
    {
        var order = new Order
        {
            Bicycles = new List<Bicycle> { new() { FreeTaxPrice = 200f } },
            Discount = 10f,
            Tax = 0f,
            ShippingCost = 0f
        };

        var total = CalculateTotal(order);

        // 200 * (1 - 10/100) = 200 * 0.9 = 180
        Assert.Equal(180f, total, precision: 2);
    }

    [Fact]
    public void Bicycle_WithTwentyPercentTax_WithTaxIncreased()
    {
        var order = new Order
        {
            Bicycles = new List<Bicycle> { new() { FreeTaxPrice = 100f } },
            Discount = 0f,
            Tax = 20f,
            ShippingCost = 0f
        };

        var total = CalculateTotal(order);

        // 100 * 1 * 1.2 = 120
        Assert.Equal(120f, total, precision: 2);
    }

    [Fact]
    public void Bicycle_WithDiscountAndTax_CorrectTotal()
    {
        var order = new Order
        {
            Bicycles = new List<Bicycle> { new() { FreeTaxPrice = 200f } },
            Discount = 10f,
            Tax = 20f,
            ShippingCost = 10f
        };

        var total = CalculateTotal(order);

        // subtotal = 200, after discount = 200*0.9 = 180, with tax = 180*1.2 = 216, + shipping = 226
        Assert.Equal(226f, total, precision: 2);
    }

    [Fact]
    public void MultipleBicycles_TotalSumsAllPrices()
    {
        var order = new Order
        {
            Bicycles = new List<Bicycle>
            {
                new() { FreeTaxPrice = 100f },
                new() { FreeTaxPrice = 200f },
                new() { FreeTaxPrice = 300f }
            },
            Discount = 0f,
            Tax = 0f,
            ShippingCost = 0f
        };

        var total = CalculateTotal(order);

        Assert.Equal(600f, total);
    }

    [Fact]
    public void Order_WithLoyaltyPointFlag_DoesNotAffectTotal()
    {
        var order = new Order
        {
            Bicycles = new List<Bicycle> { new() { FreeTaxPrice = 300f } },
            Discount = 0f,
            Tax = 0f,
            ShippingCost = 0f,
            UseLoyaltyPoint = true
        };

        var total = CalculateTotal(order);

        Assert.Equal(300f, total);
    }
}
