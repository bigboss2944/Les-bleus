using Entities;

namespace LesBleus.Tests.Unit.BusinessLogic;

/// <summary>
/// Tests unitaires pour <see cref="OrderPricingService"/>.
/// Vérifie que le calcul du total TTC est correct dans tous les cas
/// (remise, TVA, frais de port, commandes vides ou multiples vélos).
/// </summary>
public class OrderPricingServiceTests
{
    private readonly IOrderPricingService _service = new OrderPricingService();

    [Fact]
    public void EmptyOrder_TotalEqualsShippingCostOnly()
    {
        var order = new Order { ShippingCost = 15f };

        var total = _service.CalculateTotal(order);

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

        var total = _service.CalculateTotal(order);

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

        // 200 * (1 - 10/100) = 200 * 0.9 = 180
        var total = _service.CalculateTotal(order);

        Assert.Equal(180f, total, precision: 2);
    }

    [Fact]
    public void Bicycle_WithTwentyPercentTax_TotalIncreased()
    {
        var order = new Order
        {
            Bicycles = new List<Bicycle> { new() { FreeTaxPrice = 100f } },
            Discount = 0f,
            Tax = 20f,
            ShippingCost = 0f
        };

        // 100 * 1 * 1.2 = 120
        var total = _service.CalculateTotal(order);

        Assert.Equal(120f, total, precision: 2);
    }

    [Fact]
    public void Bicycle_WithDiscountAndTaxAndShipping_CorrectTotal()
    {
        var order = new Order
        {
            Bicycles = new List<Bicycle> { new() { FreeTaxPrice = 200f } },
            Discount = 10f,
            Tax = 20f,
            ShippingCost = 10f
        };

        // subtotal = 200, after discount = 200*0.9 = 180, with tax = 180*1.2 = 216, + shipping = 226
        var total = _service.CalculateTotal(order);

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

        var total = _service.CalculateTotal(order);

        Assert.Equal(600f, total);
    }

    [Fact]
    public void Order_WithFullDiscount_SubtotalIsZero()
    {
        var order = new Order
        {
            Bicycles = new List<Bicycle> { new() { FreeTaxPrice = 500f } },
            Discount = 100f,
            Tax = 20f,
            ShippingCost = 10f
        };

        // subtotal = 500, after 100% discount = 0, with tax = 0, + shipping = 10
        var total = _service.CalculateTotal(order);

        Assert.Equal(10f, total, precision: 2);
    }

    [Fact]
    public void Order_NullBicyclesList_TreatedAsZeroSubtotal()
    {
        var order = new Order
        {
            Bicycles = null!,
            Discount = 0f,
            Tax = 0f,
            ShippingCost = 5f
        };

        // Null bicycles → subtotal = 0, only shipping cost
        var total = _service.CalculateTotal(order);

        Assert.Equal(5f, total);
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

        // UseLoyaltyPoint ne modifie pas le total (logique métier non implémentée ici)
        var total = _service.CalculateTotal(order);

        Assert.Equal(300f, total);
    }
}
