using Microsoft.Extensions.Configuration;
using Minio;
using CVBuilder.Core.Interfaces;
using CVBuilder.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Infrastructure.Implementations
{
    public class AppConfiguration : IAppConfiguration
    {
        private readonly IConfiguration _config;

        public AppConfiguration(IConfiguration config)
        {
            _config = config;
        }

        public string GetKafkaBootstrapServers()
            => _config.GetValue<string>("Kafka:BootstrapServers")
               ?? throw new InvalidOperationException("Missing Kafka BootstrapServers configuration.");

        public string? GetCurrentServiceName()
            => _config.GetValue<string>("Kafka:CurrentService")
            ?? throw new InvalidOperationException("Missing Kafka CurrentService configuration.");
    }
}
