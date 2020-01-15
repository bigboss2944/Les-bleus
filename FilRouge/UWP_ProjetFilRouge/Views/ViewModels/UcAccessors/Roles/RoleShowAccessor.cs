using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_ProjetFilRouge.Entities;

namespace UWP_ProjetFilRouge.Views.ViewModels.UcAccessors.Roles
{
    public class RoleShowAccessor
    {
        public Role Role { get; set; }

        public RoleShowAccessor()
        {
            this.Role = new Role();
        }
    }
}
