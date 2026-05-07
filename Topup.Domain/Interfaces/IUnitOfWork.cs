namespace Topup.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveAysnc();
    }
}
