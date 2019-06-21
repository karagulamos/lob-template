using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using Business.Core.Entities.Identity;
using Business.Persistence.Repository.Common.Extensions;

namespace Business.Persistence.Repository.EntityConfiguration
{
    internal class RoleConfiguration : EntityTypeConfiguration<Role>, IEntityConfiguration
    {
        public RoleConfiguration()
        {
            Ignore(r => r.Id);
            HasKey(r => r.RoleId);

            Property(u => u.Name)
                .HasMaxLength(100)
                .IsUnique<Role, string>(u => u.Name)
                .IsRequired();

            HasMany(u => u.Users)
                .WithMany(r => r.Roles);
        }

        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}