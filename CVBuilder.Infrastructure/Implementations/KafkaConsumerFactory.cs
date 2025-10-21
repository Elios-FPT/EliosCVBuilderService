using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CVBuilder.Core.Interfaces;
using CVBuilder.Infrastructure.Implementations;
using CVBuilder.Infrastructure.Kafka;

public class KafkaConsumerFactory<T> : IKafkaConsumerFactory<T> where T : class
{
    private readonly IServiceProvider _serviceProvider;

    public KafkaConsumerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IKafkaConsumerRepository<T> CreateConsumer(string sourceServiceName)
    {
        return ActivatorUtilities.CreateInstance<KafkaConsumerRepository<T>>(
            _serviceProvider,
            _serviceProvider,
            _serviceProvider.GetRequiredService<IConfiguration>(),
            _serviceProvider.GetRequiredService<IAppConfiguration>(),
            _serviceProvider.GetRequiredService<IKafkaResponseHandler<T>>(),
            sourceServiceName
        );
    }
}