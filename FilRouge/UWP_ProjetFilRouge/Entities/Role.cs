using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_ProjetFilRouge.Entities
{
    [Table("role")]
    public class Role : EntityBase
    {
        private String name;
        private long id;
       

        [Column("name")]
        [Unique]
        [NotNull]
        public String Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }



        public override object Copy()
        {
            Role role = new Role();
            role.Id = this.Id;
            role.Name = this.Name;


            return role;
        }

        public override void CopyFrom(object obj)
        {
            Role role = obj as Role;
            this.Id = role.Id;
            this.Name = role.Name;

        }
    }
}
