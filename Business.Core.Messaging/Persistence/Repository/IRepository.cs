using System.Threading.Tasks;

namespace Business.Core.Messaging.Persistence.Repository
{
    internal interface IRepository<in TEntity> : IRepository where TEntity : class
    {
        void Add(TEntity entity);
        void Reattach(TEntity entity);
        void CommitChanges();
        Task CommitChangesAsync();
    }

    internal interface IRepository { }
}
