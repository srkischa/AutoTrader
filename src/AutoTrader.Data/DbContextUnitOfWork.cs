using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTrader.Data
{
    public class DbContextUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public DbContextUnitOfWork(DbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            _context = dbContext;
        }

        public void CancelSaving()
        {
            throw new NotImplementedException();
        }

        public void SaveChanges(bool force = false)
        {
            if (force)
            {
                _context.ChangeTracker.DetectChanges();
            }

            try
            {
                var numberOfChanges = _context.SaveChanges();
                //_log.DebugFormat("{0} of changed were saved to database {1}", numberOfChanges, _context.Database.Connection.Database);
            }
            catch (DbEntityValidationException ex)
            {
                LogAndThrow(ex);
            }
        }

        public async Task SaveChangesAsync(bool force = false)
        {
            if (force)
            {
                _context.ChangeTracker.DetectChanges();
            }

            try
            {
                var numberOfChanges = await _context.SaveChangesAsync();
                //_log.DebugFormat("{0} of changed were saved to database {1}", numberOfChanges, _context.Database.Connection.Database);
            }
            catch (DbEntityValidationException ex)
            {
                LogAndThrow(ex);
            }
        }

        private void LogAndThrow(DbEntityValidationException ex)
        {
            var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            var fullErrorMessage = string.Join("; ", errorMessages);

            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
            //_log.ErrorFormat("Message:\n{0}", exceptionMessage);

            throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
        }
    }
}
