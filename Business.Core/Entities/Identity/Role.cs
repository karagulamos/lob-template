using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Business.Core.Entities.Identity
{
    public class Role : IRole<short>
    {
        public Role(string roleName) : this()
        {
            Name = roleName;
        }

        public Role()
        {
            Users = new HashSet<User>();
        }

        public short Id => RoleId;

        public short RoleId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}