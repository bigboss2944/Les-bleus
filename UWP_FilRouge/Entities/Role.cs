using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UWP_FilRouge.Entities
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
            role.id = this.id;
            role.Name = this.Name;

            return role;
        }

        public override void CopyFrom(object obj)
        {
            Role role = obj as Role;
            this.id = role.id;
            this.Name = role.Name;
        }
    }
}