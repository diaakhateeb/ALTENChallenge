using DataDomainService.GenericsContext.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataDomainService.GenericsContext.Handlers
{
    /// <summary>
    /// Unit of Work class.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        /// <summary>
        /// Creates Unit of Work instance.
        /// </summary>
        /// <param name="dbContext">Database context object.</param>
        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Save entity changes to database.
        /// </summary>
        /// <returns>Returns entity Id.</returns>
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
        /// <summary>
        /// Save entity changes to database asynchronously.
        /// </summary>
        /// <returns>Returns task object.</returns>
        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
