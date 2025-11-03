using CVBuilder.Core.Interfaces;

namespace CVBuilder.Infrastructure.Kafka
{
    public interface IKafkaConsumerFactory<T> where T : class
    {
        IKafkaConsumerRepository<T> CreateConsumer(string sourceServiceName);
    }
}