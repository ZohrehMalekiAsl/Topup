namespace Topup.Domain.Models
{
    public class LogData<TRequest, TResponse>
    {
        public TRequest RequestInfo { get; set; }
        public TResponse ResponseInfo { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public string CorrelationId { get; set; }
        public string Description { get; set; }
    }
}
