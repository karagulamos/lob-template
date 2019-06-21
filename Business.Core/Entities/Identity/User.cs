using System;
using System.Collections.Generic;
using Business.Core.Common.Enums;
using Business.Core.Persistence;
using Microsoft.AspNet.Identity;

namespace Business.Core.Entities.Identity
{
    public class User : IUser<long>, IAuditable
    {
        public User()
        {
            Roles = new HashSet<Role>();
        }

        public long Id => UserId;

        public long UserId { get; set; }

        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsFirstTimeLogin { get; set; }
        public string PhoneNumber { get; set; }
        public string OptionalPhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
