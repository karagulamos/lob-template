using System.ComponentModel.Composition;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using Business.Core.Entities;
using Business.Persistence.Repository.Common.Extensions;

namespace Business.Persistence.Repository.EntityConfiguration
{
    [Export(typeof(IEntityConfiguration))]
    internal class ErrorCodeConfiguration : EntityTypeConfiguration<ErrorCode>, IEntityConfiguration
    {
        public ErrorCodeConfiguration()
        {
            Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnique<ErrorCode, string>(e => e.Code)
                .IsRequired();
        }

        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}
