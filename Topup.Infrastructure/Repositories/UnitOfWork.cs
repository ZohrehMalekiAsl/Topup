using Topup.Domain.Interfaces;
using Topup.Infrastructure.Context;

namespace Topup.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> SaveAysnc()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
