using System.Data.Entity.ModelConfiguration;
using Business.Core.Persistence;

namespace Business.Persistence.Repository.EntityConfiguration
{
    internal abstract class AuditableEntityTypeConfiguration<TEntity> : EntityTypeConfiguration<TEntity>
    where TEntity : class, IAuditable
    {
        protected AuditableEntityTypeConfiguration()
        {
            Property(r => r.DateCreated).HasColumnType("datetime2");
            Property(r => r.DateModified).HasColumnType("datetime2");
        }
    }
}