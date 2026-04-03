using Entities;

namespace LesBleus.Tests.Unit.Entities;

public class PhysicalProductTests
{
    [Fact]
    public void NewPhysicalProduct_HasNoProductTypeAssigned()
    {
        var pp = new PhysicalProduct();

        Assert.Null(pp.ProductType);
        Assert.Equal(0, pp.ProductTypeId);
    }

    [Fact]
    public void PhysicalProduct_AssignProductType_ReferenceUpdated()
    {
        var pt = new ProductType { Id = 7, Reference = "REF-001" };
        var pp = new PhysicalProduct { ProductType = pt, ProductTypeId = pt.Id };

        Assert.NotNull(pp.ProductType);
        Assert.Equal(7, pp.ProductTypeId);
        Assert.Equal("REF-001", pp.ProductType!.Reference);
    }

    [Fact]
    public void PhysicalProduct_OnlyOneProductTypeAtATime()
    {
        var firstType = new DeliverableProduct { Id = 1, Reference = "DEL-001" };
        var secondType = new InsuredProduct { Id = 2, Reference = "INS-001" };
        var pp = new PhysicalProduct { ProductType = firstType, ProductTypeId = firstType.Id };

        pp.ProductType = secondType;
        pp.ProductTypeId = secondType.Id;

        Assert.Equal(2, pp.ProductTypeId);
        Assert.Equal("INS-001", pp.ProductType!.Reference);
    }

    [Fact]
    public void PhysicalProduct_ToString_ContainsId()
    {
        var pp = new PhysicalProduct { Id = 42 };

        Assert.Contains("42", pp.ToString());
    }
}
