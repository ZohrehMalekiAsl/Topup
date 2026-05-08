using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Application.Interfaces.Infra;

namespace Topup.Infrastructure
{
    public class AppSetting 
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public ConsumerMq RabbitMq { get; set; }=new ConsumerMq();
        public PublisherMq PublisherRabbitMq { get; set; }=new PublisherMq();
        public string RetryCount { get; set; }
        public string HamrahAvalUrl { get; set; }

    }

    public class ConsumerMq
    {
        public string ConsumerQueue { get; set; }
        public string ConsumerMqHostName { get; set; }
    }
    public class PublisherMq
    {
       public string SuccessPublisherMqHostName { get; set; }
       public string SuccessPublisherQueue { get; set; }
       public string FailPublisherMqHostName { get; set; }
       public string FailPublisherQueue { get; set; }
    }
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
}
