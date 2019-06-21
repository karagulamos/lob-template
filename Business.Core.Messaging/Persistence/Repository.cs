using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text;
using System.Threading.Tasks;
using Business.Core.Messaging.Persistence.Repository;

namespace Business.Core.Messaging.Persistence
{
    internal abstract class Repository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : class
    where TContext : DbContext, new()
    {
        protected TContext Context;

        private readonly DbSet<TEntity> _entitySet;

        protected Repository(TContext context)
        {
            Context = context;
            _entitySet = context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            _entitySet.Add(entity);
        }

        public void CommitChanges()
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Context.SaveChanges();
                    transaction.Commit();
                }
                catch (DbEntityValidationException devex)
                {
                    var outputLines = new StringBuilder();

                    foreach (var eve in devex.EntityValidationErrors)
                    {
                        outputLines.AppendLine(
                            $"{DateTime.Now}: Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors: ");

                        foreach (var ve in eve.ValidationErrors)
                        {
                            outputLines.AppendLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                        }
                    }

                    transaction.Rollback();

                    throw new DbEntityValidationException(outputLines.ToString(), devex);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task CommitChangesAsync()
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    await Context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (DbEntityValidationException devex)
                {
                    var outputLines = new StringBuilder();

                    foreach (var eve in devex.EntityValidationErrors)
                    {
                        outputLines.AppendLine(
                            $"{DateTime.Now}: Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors: ");

                        foreach (var ve in eve.ValidationErrors)
                        {
                            outputLines.AppendLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                        }
                    }

                    transaction.Rollback();

                    throw new DbEntityValidationException(outputLines.ToString(), devex);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Reattach(TEntity entity)
        {
            var entry = Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                _entitySet.Attach(entity);
            }
        }
    }
}
