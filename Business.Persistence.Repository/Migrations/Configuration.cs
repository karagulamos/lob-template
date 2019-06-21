using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Business.Core.Common.Enums;
using Business.Core.Common.Extensions;
using Business.Core.Entities.Identity;

namespace Business.Persistence.Repository.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<BusinessDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BusinessDataContext context)
        {
            //DropUserRoles(context);
            //DropUsers(context);
            //DropRoles(context);

            SeedUserRoles(context);
            context.SaveChanges();

            const string userName = "admin@business.com";

            if (context.Users.Any(u => u.Email == userName || u.UserName == userName))
                return;

            var user = new User
            {
                Email = userName,
                UserName = userName,
                PasswordHash = "password".Encrypt()
            };

            var role = context.Roles.SingleOrDefault(r => r.Name == "Administrator") ?? new Role("Administrator");

            user.Roles.Add(role);

            context.Users.Add(user);

            context.SaveChanges();
        }

        #region Table Cleanup
        private static void DropUsers(DbContext context)
        {
            context.Database.ExecuteSqlCommand($"delete from {nameof(User)}s");
        }

        private static void DropRoles(DbContext context)
        {
            context.Database.ExecuteSqlCommand($"delete from {nameof(Role)}s");
        }

        private static void DropUserRoles(DbContext context)
        {
            context.Database.ExecuteSqlCommand($"truncate table {nameof(User)}{nameof(Role)}s");
        }
        #endregion

        private static void SeedUserRoles(BusinessDataContext context)
        {
            if (context.Roles.Any())
                return;

            var roles = Enum.GetNames(typeof(UserType))
                            .Select(ut => new Role(ut));

            context.Roles.AddRange(roles);
        }
    }
}
