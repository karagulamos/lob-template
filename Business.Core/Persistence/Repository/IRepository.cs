namespace Business.Core.Persistence.Repository
{
    public interface IRepository<TEntity> : IRepositoryReadOnly<TEntity>, IRepositoryWriteOnly<TEntity> where TEntity : class
    {
    }
}
