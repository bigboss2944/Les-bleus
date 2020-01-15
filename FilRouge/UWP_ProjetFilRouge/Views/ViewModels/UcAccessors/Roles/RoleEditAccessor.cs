using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_ProjetFilRouge.Entities;
using UWP_ProjetFilRouge.Views.ViewModels.UcAccessors.Commons;

namespace UWP_ProjetFilRouge.Views.ViewModels.UcAccessors.Roles
{
    public class RoleEditAccessor
    {
        public Role Role { get; set; }
        public ButtonAccessor Button { get; set; }

        public RoleEditAccessor()
        {
            this.Role = new Role();
            this.Button = new ButtonAccessor();
        }
    }
}
