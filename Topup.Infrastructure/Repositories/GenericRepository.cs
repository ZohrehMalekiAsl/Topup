using Topup.Domain.Entities;
using Topup.Infrastructure.Context;

namespace Topup.Infrastructure.Repositories
{
    public class GenericRepository<TEntity>(ApplicationDbContext dbContext) where TEntity : IEntity
    {
        public void Add(TEntity entity)
        {
            dbContext.Add(entity);
        }
        public void Update(TEntity entity)
        {
            dbContext.Update(entity);
        }
        public void Delete(TEntity entity)
        {
            dbContext.Remove(entity);
        }
    }
}
