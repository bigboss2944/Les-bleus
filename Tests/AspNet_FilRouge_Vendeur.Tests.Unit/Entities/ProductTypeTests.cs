using Entities;

namespace LesBleus.Tests.Unit.Entities;

public class ProductTypeTests
{
    [Fact]
    public void NewProductType_DefaultValues_AreZeroOrNull()
    {
        var pt = new ProductType();

        Assert.NotNull(pt.Characteristics);
        Assert.Equal(0f, pt.Characteristics.Size);
        Assert.Equal(0f, pt.Characteristics.Weight);
        Assert.Null(pt.Characteristics.Color);
        Assert.Null(pt.Reference);
        Assert.Equal(0f, pt.FreeTaxPrice);
        Assert.Equal(0f, pt.Tax);
    }

    [Fact]
    public void ProductType_Size_SetAndGet()
    {
        var pt = new ProductType { Size = 54.5f };

        Assert.Equal(54.5f, pt.Size);
        Assert.Equal(54.5f, pt.Characteristics.Size);
    }

    [Fact]
    public void ProductType_Weight_SetAndGet()
    {
        var pt = new ProductType { Weight = 8.2f };

        Assert.Equal(8.2f, pt.Weight);
        Assert.Equal(8.2f, pt.Characteristics.Weight);
    }

    [Fact]
    public void ProductType_Color_SetAndGet()
    {
        var pt = new ProductType { Color = "Rouge" };

        Assert.Equal("Rouge", pt.Color);
        Assert.Equal("Rouge", pt.Characteristics.Color);
    }

    [Fact]
    public void ProductType_Characteristics_ObjectCanBeAssigned()
    {
        var pt = new ProductType
        {
            Characteristics = new ProductCharacteristics
            {
                Size = 56f,
                Weight = 9.1f,
                Color = "Noir"
            }
        };

        Assert.Equal(56f, pt.Size);
        Assert.Equal(9.1f, pt.Weight);
        Assert.Equal("Noir", pt.Color);
    }

    [Fact]
    public void ProductType_Reference_SetAndGet()
    {
        var pt = new ProductType { Reference = "REF-PT-001" };

        Assert.Equal("REF-PT-001", pt.Reference);
    }

    [Fact]
    public void ProductType_FreeTaxPrice_SetAndGet()
    {
        var pt = new ProductType { FreeTaxPrice = 499.99f };

        Assert.Equal(499.99f, pt.FreeTaxPrice);
    }

    [Fact]
    public void ProductType_Tax_SetAndGet()
    {
        var pt = new ProductType { Tax = 20f };

        Assert.Equal(20f, pt.Tax);
    }

    [Fact]
    public void ProductType_ToString_ContainsReference()
    {
        var pt = new ProductType { Reference = "REF-001", FreeTaxPrice = 100f };
        var result = pt.ToString();

        Assert.Contains("REF-001", result);
    }

    [Fact]
    public void DeliverableProduct_IsProductType()
    {
        var dp = new DeliverableProduct { FreeTaxPrice = 300f };

        Assert.IsAssignableFrom<ProductType>(dp);
        Assert.Equal(300f, dp.FreeTaxPrice);
    }

    [Fact]
    public void InsuredProduct_IsProductType()
    {
        var ip = new InsuredProduct { Tax = 20f };

        Assert.IsAssignableFrom<ProductType>(ip);
        Assert.Equal(20f, ip.Tax);
    }

    [Fact]
    public void ExchangeableProduct_IsProductType()
    {
        var ep = new ExchangeableProduct { Color = "Bleu" };

        Assert.IsAssignableFrom<ProductType>(ep);
        Assert.Equal("Bleu", ep.Color);
    }
}
