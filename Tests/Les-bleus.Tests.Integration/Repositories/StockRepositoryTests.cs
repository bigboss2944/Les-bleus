using AspNet_FilRouge.Models;
using Microsoft.EntityFrameworkCore;

namespace LesBleus.Tests.Integration.Repositories;

public class StockRepositoryTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task AddStock_CanBeRetrievedById()
    {
        using var context = CreateContext();
        var pt = new ProductType { Reference = "REF-001", FreeTaxPrice = 299f };
        context.ProductTypes.Add(pt);
        await context.SaveChangesAsync();

        var stock = new Stock { ProductTypeId = pt.Id, Quantity = 10 };
        context.Stocks.Add(stock);
        await context.SaveChangesAsync();

        var retrieved = await context.Stocks.FindAsync(stock.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(10, retrieved.Quantity);
    }

    [Fact]
    public async Task UpdateStock_Quantity_PersistsChange()
    {
        using var context = CreateContext();
        var pt = new ProductType { Reference = "REF-002" };
        context.ProductTypes.Add(pt);
        await context.SaveChangesAsync();

        var stock = new Stock { ProductTypeId = pt.Id, Quantity = 5 };
        context.Stocks.Add(stock);
        await context.SaveChangesAsync();

        stock.Quantity = 20;
        await context.SaveChangesAsync();

        var retrieved = await context.Stocks.FindAsync(stock.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(20, retrieved.Quantity);
    }

    [Fact]
    public async Task DeleteStock_IsRemovedFromContext()
    {
        using var context = CreateContext();
        var pt = new ProductType { Reference = "REF-003" };
        context.ProductTypes.Add(pt);
        await context.SaveChangesAsync();

        var stock = new Stock { ProductTypeId = pt.Id, Quantity = 8 };
        context.Stocks.Add(stock);
        await context.SaveChangesAsync();

        context.Stocks.Remove(stock);
        await context.SaveChangesAsync();

        var retrieved = await context.Stocks.FindAsync(stock.Id);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task AddMultipleStocks_CountIsCorrect()
    {
        using var context = CreateContext();
        var pt1 = new ProductType { Reference = "REF-A" };
        var pt2 = new ProductType { Reference = "REF-B" };
        var pt3 = new ProductType { Reference = "REF-C" };
        context.ProductTypes.AddRange(pt1, pt2, pt3);
        await context.SaveChangesAsync();

        context.Stocks.AddRange(
            new Stock { ProductTypeId = pt1.Id, Quantity = 1 },
            new Stock { ProductTypeId = pt2.Id, Quantity = 2 },
            new Stock { ProductTypeId = pt3.Id, Quantity = 3 });
        await context.SaveChangesAsync();

        var count = await context.Stocks.CountAsync();

        Assert.Equal(3, count);
    }
}
