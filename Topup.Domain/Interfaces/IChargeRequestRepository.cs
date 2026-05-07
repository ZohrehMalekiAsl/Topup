using Topup.Domain.Entities;

namespace Topup.Domain.Repositories
{
    public interface IChargeRequestRepository
    {
        void Add(ChargeRequest request);
        void Update(ChargeRequest request);
        Task<ChargeRequest> GetRequestById(Guid guid);
        Task<List<ChargeRequest>> GetRequestByStatus(string status);
    }
}
