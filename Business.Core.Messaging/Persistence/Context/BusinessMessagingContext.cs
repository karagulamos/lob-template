using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data.Entity;
using System.Reflection;
using System.Threading.Tasks;
using Business.Core.Messaging.Config;
using Business.Core.Messaging.Entities;
using Business.Core.Messaging.Persistence.Context.Configuration;

namespace Business.Core.Messaging.Persistence.Context
{
    internal class BusinessMessagingContext : DbContext
    {
        public BusinessMessagingContext() : base(SmtpConfig.Get().DbNode.ConnectionString ?? "MessageX")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BusinessMessagingContext, Migrations.Configuration>());
        }

        public DbSet<OutboundSmtp> OutboundSmtpDetails { get; set; }
        public DbSet<OutboundEmail> OutboundEmails { get; set; }
        public DbSet<OutboundEmailRecipient> OutboundEmailRecipients { get; set; }
        public DbSet<OutboundAttachment> OutboundAttachments { get; set; }
        public DbSet<OutboundImage> OutboundImages { get; set; }

        public DbSet<OutboundSms> OutboundSmses { get; set; }
        public DbSet<OutboundSmsRecipient> OutboundSmsRecipients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var contextConfiguration = new ContextConfiguration();
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(catalog);

            container.ComposeParts(contextConfiguration);

            foreach (var configuration in contextConfiguration.Configurations)
            {
                configuration.AddConfiguration(modelBuilder.Configurations);
            }

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            PerformEntityAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            PerformEntityAudit();
            return base.SaveChangesAsync();
        }
        
        private void PerformEntityAudit()
        {
            foreach (var entry in ChangeTracker.Entries<ITrackable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        var currentDateTime = DateTime.Now;
                        entry.Entity.DateCreated = currentDateTime;
                        entry.Entity.DateModified = currentDateTime;
                        break;

                    case EntityState.Modified:
                        entry.Entity.DateModified = DateTime.Now;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;

                        entry.Entity.DateModified = DateTime.Now;
                        break;
                }
            }
        }
    }
}
