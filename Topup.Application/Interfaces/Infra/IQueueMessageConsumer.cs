namespace Topup.Application.Interfaces.Infra
{
    public interface IQueueMessageConsumer
    {
        void StartConsuming(CancellationToken ct);
    }
}
