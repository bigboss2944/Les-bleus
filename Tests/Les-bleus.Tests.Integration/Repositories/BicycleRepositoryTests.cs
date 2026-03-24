using Entities;
using Microsoft.EntityFrameworkCore;

namespace LesBleus.Tests.Integration.Repositories;

public class BicycleRepositoryTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task AddBicycle_CanBeRetrievedById()
    {
        using var context = CreateContext();
        var bike = new Bicycle { TypeOfBike = "Road", FreeTaxPrice = 499f };
        context.Bicycles.Add(bike);
        await context.SaveChangesAsync();

        var retrieved = await context.Bicycles.FindAsync(bike.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("Road", retrieved.TypeOfBike);
    }

    [Fact]
    public async Task AddMultipleBicycles_CountIsCorrect()
    {
        using var context = CreateContext();
        context.Bicycles.AddRange(
            new Bicycle { TypeOfBike = "Road" },
            new Bicycle { TypeOfBike = "Mountain" },
            new Bicycle { TypeOfBike = "City" });
        await context.SaveChangesAsync();

        var count = await context.Bicycles.CountAsync();

        Assert.Equal(3, count);
    }

    [Fact]
    public async Task DeleteBicycle_IsRemovedFromContext()
    {
        using var context = CreateContext();
        var bike = new Bicycle { TypeOfBike = "Road" };
        context.Bicycles.Add(bike);
        await context.SaveChangesAsync();

        context.Bicycles.Remove(bike);
        await context.SaveChangesAsync();

        var retrieved = await context.Bicycles.FindAsync(bike.Id);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task UpdateBicycle_FreeTaxPrice_PersistsChange()
    {
        using var context = CreateContext();
        var bike = new Bicycle { TypeOfBike = "Road", FreeTaxPrice = 300f };
        context.Bicycles.Add(bike);
        await context.SaveChangesAsync();

        bike.FreeTaxPrice = 450f;
        await context.SaveChangesAsync();

        var retrieved = await context.Bicycles.FindAsync(bike.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(450f, retrieved.FreeTaxPrice);
    }
}
