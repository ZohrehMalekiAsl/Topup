using Microsoft.EntityFrameworkCore;
using Topup.Domain.Entities;
using Topup.Domain.Repositories;
using Topup.Infrastructure.Context;

namespace Topup.Infrastructure.Repositories
{
    public class ChargeRequestRepository(ApplicationDbContext dbContext) : GenericRepository<ChargeRequest>(dbContext), IChargeRequestRepository
    {
        public async Task<ChargeRequest> GetRequestById(Guid guid)
        {
            return await dbContext.ChargeRequest.FindAsync(guid);
        }

        public Task<List<ChargeRequest>> GetRequestByStatus(string status)
        {
            throw new NotImplementedException();
        }
    }
}
