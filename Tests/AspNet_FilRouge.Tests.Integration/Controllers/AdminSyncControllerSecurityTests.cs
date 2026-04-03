using System.Reflection;
using AspNet_FilRouge.Controllers.Api;
using Entities;
using Microsoft.AspNetCore.Authorization;

namespace LesBleus.Tests.Integration.Controllers;

public class AdminSyncControllerSecurityTests
{
    [Fact]
    public void SyncController_RequiresAdministrateurRole()
    {
        var authorizeAttribute = typeof(SyncController)
            .GetCustomAttributes<AuthorizeAttribute>(inherit: true)
            .SingleOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal(AppConstants.Roles.Administrateur, authorizeAttribute!.Roles);
    }
}
