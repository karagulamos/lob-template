using System.Data.Entity;
using Business.Persistence.Repository.Interceptors;

namespace Business.Persistence.Repository
{
    public class EntityFrameworkConfiguration : DbConfiguration
    {
        public EntityFrameworkConfiguration()
        {
            AddInterceptor(new SoftDeleteInterceptor());
        }
    }
}