using System.Reflection;
using AspNet_FilRouge_Vendeur.Controllers.Api;
using Entities;
using Microsoft.AspNetCore.Authorization;

namespace LesBleus.Tests.Integration.Controllers;

public class VendorSyncControllerSecurityTests
{
    [Fact]
    public void SyncController_RequiresAdministrateurOrVendeurRole()
    {
        var authorizeAttribute = typeof(SyncController)
            .GetCustomAttributes<AuthorizeAttribute>(inherit: true)
            .SingleOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal(AppConstants.Roles.Administrateur + "," + AppConstants.Roles.Vendeur, authorizeAttribute!.Roles);
    }
}
