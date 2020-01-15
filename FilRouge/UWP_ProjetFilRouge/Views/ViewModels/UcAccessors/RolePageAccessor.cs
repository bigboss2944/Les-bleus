using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_ProjetFilRouge.Views.ViewModels.UcAccessors.Roles;

namespace UWP_ProjetFilRouge.Views.ViewModels.UcAccessors
{
    public class RolePageAccessor
    {
        public RoleEditAccessor RoleEdit { get; set; }
        public RoleListAccessor RoleList { get; set; }
        public RoleShowAccessor RoleShow { get; set; }

        public RolePageAccessor()
        {
            this.RoleEdit = new RoleEditAccessor();
            this.RoleList = new RoleListAccessor();
            this.RoleShow = new RoleShowAccessor();
        }
    }
}
