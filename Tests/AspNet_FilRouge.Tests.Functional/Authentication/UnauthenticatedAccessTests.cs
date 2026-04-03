using AspNet_FilRouge.Tests.Functional.Helpers;

namespace AspNet_FilRouge.Tests.Functional.Authentication;

public class UnauthenticatedAccessTests : IClassFixture<AdminWebApplicationFactory>
{
    private readonly AdminWebApplicationFactory _factory;

    public UnauthenticatedAccessTests(AdminWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/Bicycles")]
    [InlineData("/Orders")]
    [InlineData("/StockRequests")]
    [InlineData("/Customers")]
    [InlineData("/Sellers")]
    public async Task UnauthenticatedRequest_RedirectsToLogin(string url)
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync(url);

        Assert.True((int)response.StatusCode is >= 300 and < 400,
            $"Expected redirect for {url}, got {(int)response.StatusCode}");
        Assert.Contains("Account/Login", response.Headers.Location?.ToString() ?? string.Empty);
    }
}
