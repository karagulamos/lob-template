using System.Data.Entity.ModelConfiguration.Configuration;

namespace Business.Persistence.Repository.EntityConfiguration
{
    internal interface IEntityConfiguration
    {
        void AddConfiguration(ConfigurationRegistrar registrar);
    }
}