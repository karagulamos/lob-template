using System.ComponentModel.Composition;
using System.Data.Entity.ModelConfiguration.Configuration;
using Business.Core.Messaging.Common.Helpers;
using Business.Core.Messaging.Entities;
using Business.Core.Messaging.Persistence.Context.Configuration;

namespace Business.Core.Messaging.Persistence.Repository.EntityConfiguration
{
    [Export(typeof(IEntityConfiguration))]
    internal class OutboundSmtpConfiguration : TrackableEntityTypeConfiguration<OutboundSmtp>, IEntityConfiguration
    {
        public OutboundSmtpConfiguration()
        {
            Property(e => e.DescriptorId)
                .IsRequired()
                .IsUnique<OutboundSmtp, int>(e => e.DescriptorId);
        }

        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}
