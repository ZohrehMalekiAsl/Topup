namespace Topup.Application.Interfaces.Infra
{
    public interface IQMConsumerService
    {
        Task StartConsuming(CancellationToken ct);
    }
}
