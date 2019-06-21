using System;
using System.Data.Entity.Validation;
using System.Text;
using System.Threading.Tasks;
using Business.Core.Persistence;
using Business.Core.Persistence.Repository;
using Ninject;

namespace Business.Persistence.Repository
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : BusinessDataContext
    {
        [Inject]
        public IErrorCodeRepository ErrorCodes { get; set; }

        #region Base Setup

        private readonly TContext _context;

        public UnitOfWork(TContext context)
        {
            _context = context;
        }
        #endregion

        #region Base Implementations
        public async Task CompleteAsync()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync();
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

        public void Dispose()
        {
            Dispose(true);
        }

        private volatile bool _isDisposing;

        private void Dispose(bool disposing)
        {
            if (_isDisposing) return;

            _isDisposing = disposing;
            _context.Dispose();
            _isDisposing = false;
        }

        #endregion
    }
}
