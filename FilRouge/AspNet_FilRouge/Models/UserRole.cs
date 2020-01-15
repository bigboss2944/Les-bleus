using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet_FilRouge.Models
{
    public class UserRole
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public void AddUserToRole(string userName, string roleName)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            try
            {
                var user = UserManager.FindByName(userName);
                UserManager.AddToRole(user.Id, roleName);
                           }
            catch
            {
                throw;
            }
        }
    }
}