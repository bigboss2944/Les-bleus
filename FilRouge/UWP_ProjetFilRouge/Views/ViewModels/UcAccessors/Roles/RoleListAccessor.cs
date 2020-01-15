using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_ProjetFilRouge.Entities;
using UWP_ProjetFilRouge.Views.ViewModels.UcAccessors.Commons;

namespace UWP_ProjetFilRouge.Views.ViewModels.UcAccessors.Roles
{
    public class RoleListAccessor
    {
        public ObservableCollection<Role> Roles { get; set; }
        public ListViewAccessor<Role> ListView { get; set; }

        public RoleListAccessor()
        {
            this.Roles = new ObservableCollection<Role>();
            this.ListView = new ListViewAccessor<Role>();
        }
    }
}
