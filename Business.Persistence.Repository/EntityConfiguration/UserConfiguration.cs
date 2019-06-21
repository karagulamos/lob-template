using System.ComponentModel.Composition;
using System.Data.Entity.ModelConfiguration.Configuration;
using Business.Core.Entities.Identity;
using Business.Persistence.Repository.Common.Extensions;

namespace Business.Persistence.Repository.EntityConfiguration
{
    [Export(typeof(IEntityConfiguration))]
    internal class UserConfiguration : AuditableEntityTypeConfiguration<User>, IEntityConfiguration
    {
        public UserConfiguration()
        {
            Ignore(r => r.Id);
            HasKey(r => r.UserId);

            Property(r => r.Email)
                .HasMaxLength(100)
                .IsUnique<User, string>(u => u.Email)
                .IsRequired();

            Property(r => r.UserName)
               .HasMaxLength(100)
               .IsUnique<User, string>(u => u.UserName);

            HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .Map(r => r.MapLeftKey($"{nameof(User)}Id")
                           .MapRightKey($"{nameof(Role)}Id"));
        }

        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}
