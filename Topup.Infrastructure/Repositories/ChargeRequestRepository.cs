using Microsoft.EntityFrameworkCore;
using System;
using Topup.Domain.Entities;
using Topup.Domain.Enums;
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

        public async Task<List<ChargeRequest>> GetRequestByStatus(string status)
        {
            return await dbContext.ChargeRequest
                .Where(x=> x.Status == status)
                .Take(40)
                .ToListAsync();
             
        }
    }
}
