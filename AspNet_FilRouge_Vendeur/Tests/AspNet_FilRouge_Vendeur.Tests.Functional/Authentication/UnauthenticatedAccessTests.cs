using LesBleus.Tests.Functional.Helpers;

namespace LesBleus.Tests.Functional.Authentication;

public class UnauthenticatedAccessTests : IClassFixture<VendeurWebApplicationFactory>
{
    private readonly VendeurWebApplicationFactory _factory;

    public UnauthenticatedAccessTests(VendeurWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/Bicycles")]
    [InlineData("/Orders")]
    [InlineData("/StockRequests")]
    public async Task UnauthenticatedRequest_RedirectsToLogin(string url)
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync(url);

        // Unauthenticated requests should redirect (301/302) to login
        Assert.True(
            (int)response.StatusCode is >= 300 and < 400,
            $"Expected redirect for {url}, got {(int)response.StatusCode}");
        Assert.Contains("Account/Login", response.Headers.Location?.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task LoginPage_IsAccessibleWithoutAuthentication()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Account/Login");

        Assert.True(
            (int)response.StatusCode is 200 or >= 300 and < 400,
            $"Login page returned unexpected status {(int)response.StatusCode}");
    }
}
