using GIGMS.Core.Entities.Inventory;
using System.ComponentModel.Composition;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace GIGMS.Persistence.Repository.EntityConfiguration
{
    [Export(typeof(IEntityConfiguration))]
    internal class StockSupplyDetailConfiguration : AuditableEntityTypeConfiguration<StockSupplyDetail>, IEntityConfiguration
    {
        public StockSupplyDetailConfiguration()
        {


            HasRequired(s => s.StockRequest)
          .WithMany()
          .HasForeignKey(s => s.StockRequestId)
          .WillCascadeOnDelete(false);




        }

        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}
