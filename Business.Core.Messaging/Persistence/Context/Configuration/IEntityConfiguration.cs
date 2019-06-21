using System.Data.Entity.ModelConfiguration.Configuration;

namespace Business.Core.Messaging.Persistence.Context.Configuration
{
    internal interface IEntityConfiguration
    {
        void AddConfiguration(ConfigurationRegistrar registrar);
    }
}