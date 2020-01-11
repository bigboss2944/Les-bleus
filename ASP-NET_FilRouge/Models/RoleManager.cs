using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP_NET_FilRouge.Models
{
    public class RoleManager
    {
        public static IdentityRole CreateOrGetRole(string roleName)
        {
            EntitiesContext context = new EntitiesContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            IdentityRole role = null;

            if (!roleManager.RoleExists(roleName))
            {
                role = new IdentityRole();
                role.Name = roleName;
                roleManager.Create(role);
            }

            return role ?? roleManager.FindByName(roleName);
        }
    }
}