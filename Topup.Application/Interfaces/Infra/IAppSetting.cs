namespace Topup.Application.Interfaces.Infra
{
    public interface IAppSetting
    {
        string ConnectionString { get; }
        string ConsumerMqHostName { get; }
        string ConsumerQueue { get; }
        string SuccessPublisherMqHostName { get; }
        string SuccessPublisherQueue { get; }
        string FailPublisherMqHostName { get; }
        string FailPublisherQueue { get; }
    }
}
