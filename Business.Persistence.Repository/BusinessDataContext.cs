using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data.Entity;
using System.Reflection;
using System.Threading.Tasks;
using Business.Core.Entities;
using Business.Core.Entities.Identity;
using Business.Core.Persistence;
using Business.Persistence.Repository.EntityConfiguration;

namespace Business.Persistence.Repository
{
    [DbConfigurationType(typeof(EntityFrameworkConfiguration))]
    public class BusinessDataContext : DbContext //IdentityDbContext<AppUser>
    {
        public BusinessDataContext() : base("BusinessDataContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BusinessDataContext, Migrations.Configuration>());
        }

        public DbSet<ErrorCode> ErrorCodes { get; set; }
        public DbSet<ApiClient> ApiClients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

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
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        var currentDateTime = DateTime.Now;
                        entry.Entity.DateCreated = currentDateTime;
                        entry.Entity.DateModified = currentDateTime;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.DateModified = DateTime.Now;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;

                        entry.Entity.DateModified = DateTime.Now;
                        entry.Entity.IsDeleted = true;
                        break;
                }
            }
        }
    }
}
