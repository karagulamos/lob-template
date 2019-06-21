using System.Data.Entity.Migrations;
using Business.Core.Messaging.Persistence.Context;

namespace Business.Core.Messaging.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<BusinessMessagingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BusinessMessagingContext context)
        {
           
        }
    }
}
