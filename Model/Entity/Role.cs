using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName{ get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
