using System.Data.Entity.ModelConfiguration;
using Business.Core.Messaging.Entities;

namespace Business.Core.Messaging.Persistence.Repository.EntityConfiguration
{
    internal abstract class TrackableEntityTypeConfiguration<TEntity> : EntityTypeConfiguration<TEntity>
    where TEntity : class, ITrackable
    {
        protected TrackableEntityTypeConfiguration()
        {
            Property(r => r.DateCreated).HasColumnType("datetime2");
            Property(r => r.DateModified).HasColumnType("datetime2");
        }
    }
}
