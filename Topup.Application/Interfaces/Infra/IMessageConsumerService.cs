namespace Topup.Application.Interfaces.Infra
{
    public interface IMessageConsumerService
    {
        Task StartConsuming(CancellationToken ct);
    }
}
