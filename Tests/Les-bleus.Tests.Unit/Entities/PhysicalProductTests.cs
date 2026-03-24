using AspNet_FilRouge.Models;

namespace LesBleus.Tests.Unit.Entities;

public class PhysicalProductTests
{
    [Fact]
    public void NewPhysicalProduct_HasEmptyProductTypesList()
    {
        var pp = new PhysicalProduct();

        Assert.NotNull(pp.ProductTypes);
        Assert.Empty(pp.ProductTypes);
    }

    [Fact]
    public void PhysicalProduct_AddProductType_ListUpdated()
    {
        var pp = new PhysicalProduct();
        var pt = new ProductType { Reference = "REF-001" };

        pp.ProductTypes.Add(pt);

        Assert.Single(pp.ProductTypes);
        Assert.Equal("REF-001", pp.ProductTypes[0].Reference);
    }

    [Fact]
    public void PhysicalProduct_CanBelongToMultipleProductTypes()
    {
        var pp = new PhysicalProduct();
        pp.ProductTypes.Add(new DeliverableProduct { Reference = "DEL-001" });
        pp.ProductTypes.Add(new InsuredProduct { Reference = "INS-001" });
        pp.ProductTypes.Add(new ExchangeableProduct { Reference = "EXC-001" });

        Assert.Equal(3, pp.ProductTypes.Count);
    }

    [Fact]
    public void PhysicalProduct_ToString_ContainsId()
    {
        var pp = new PhysicalProduct { Id = 42 };

        Assert.Contains("42", pp.ToString());
    }
}
