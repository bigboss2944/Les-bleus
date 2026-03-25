using Microsoft.EntityFrameworkCore;
using Entities;

namespace LesBleus.Tests.Integration.Repositories;

public class StockRequestRepositoryTests
{
    private static Entities.ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<Entities.ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task CreateStockRequest_DefaultStatusIsEnAttente()
    {
        using var context = CreateContext();
        var request = new Entities.StockRequest
        {
            BicycleName = "Trek FX3",
            Quantity = 5
        };
        context.StockRequests.Add(request);
        await context.SaveChangesAsync();

        var retrieved = await context.StockRequests.FindAsync(request.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("En attente", retrieved.Status);
    }

    [Fact]
    public async Task UpdateStatus_ToApprouvee_PersistsChange()
    {
        using var context = CreateContext();
        var request = new Entities.StockRequest { BicycleName = "Trek FX3", Quantity = 3 };
        context.StockRequests.Add(request);
        await context.SaveChangesAsync();

        request.Status = "Approuvée";
        await context.SaveChangesAsync();

        var retrieved = await context.StockRequests.FindAsync(request.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Approuvée", retrieved.Status);
    }

    [Fact]
    public async Task UpdateStatus_ToRejetee_PersistsChange()
    {
        using var context = CreateContext();
        var request = new Entities.StockRequest { BicycleName = "Specialized Allez", Quantity = 2 };
        context.StockRequests.Add(request);
        await context.SaveChangesAsync();

        request.Status = "Rejetée";
        await context.SaveChangesAsync();

        var retrieved = await context.StockRequests.FindAsync(request.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Rejetée", retrieved.Status);
    }

    [Fact]
    public async Task RetrieveAllStockRequests_OrderedByDate()
    {
        using var context = CreateContext();
        var now = DateTime.Now;
        context.StockRequests.AddRange(
            new Entities.StockRequest { BicycleName = "A", Quantity = 1, RequestDate = now.AddDays(-2) },
            new Entities.StockRequest { BicycleName = "B", Quantity = 2, RequestDate = now.AddDays(-1) },
            new Entities.StockRequest { BicycleName = "C", Quantity = 3, RequestDate = now });
        await context.SaveChangesAsync();

        var requests = await context.StockRequests
            .OrderBy(r => r.RequestDate)
            .ToListAsync();

        Assert.Equal(3, requests.Count);
        Assert.Equal("A", requests[0].BicycleName);
        Assert.Equal("C", requests[2].BicycleName);
    }
}
