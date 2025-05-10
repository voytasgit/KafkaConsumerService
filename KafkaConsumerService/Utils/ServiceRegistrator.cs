using KafkaConsumerService.Configuration;
using KafkaConsumerService.Factories;
using KafkaConsumerService.Models;
using KafkaConsumerService.Repository;
using KafkaConsumerService.Serialization;
using KafkaConsumerService.Services;
using KafkaConsumerService.Strategien;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace KafkaConsumerService.Utils
{
    // Static class to register services with Dependency Injection container
    public static class ServiceRegistrator
    {
        // Method to register services required for the Kafka Consumer Service
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // Read appsettings.json to determine if in-memory DB should be used
            var useInMemory = string.Equals(configuration["UseInMemoryDb"] ?? "false", "true", StringComparison.OrdinalIgnoreCase);

            // If in-memory DB is to be used (e.g., for testing), set it up
            if (useInMemory)
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb")); // Use in-memory database for tests
            }
            else
            {
                // If not using in-memory DB, configure SQL Server connection string
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(configuration["ConnectionStrings:ConnectionCrmTest"])); // Use SQL Server for production
            }

            // Load Kafka configuration section from appsettings.json and register it as a singleton
            var section = configuration.GetRequiredSection("KafkaConfig");
            var kafkaConfig = new KafkaConfig
            {
                BootstrapServers = section["BootstrapServers"] ?? throw new InvalidOperationException("KafkaConfig:BootstrapServers is missing or empty."),
                Topic = section["Topic"] ?? throw new InvalidOperationException("KafkaConfig:Topic is missing."),
                ModelNameSpace = section["ModelNameSpace"] ?? throw new InvalidOperationException("ModelNameSpace is missing."),
                GroupId = section["GroupId"] ?? throw new InvalidOperationException("KafkaConfig:GroupId is missing."),
            };
            services.AddSingleton(kafkaConfig);

            // Add Serilog logging to the service container
            services.AddLogging(builder => builder.AddSerilog());

            // Register repository and services for Dependency Injection
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // Generic repository registration
            services.AddScoped<IDistlistService, DistlistService>(); // Distlist service
            services.AddScoped<IModelServiceFactory, ModelServiceFactory>(); // Model service factory

            // Register Kafka producer and outbox service as transient services
            services.AddTransient<IKafkaProducerService, KafkaProducerService>();
            services.AddTransient<IKafkaOutboxService, KafkaOutboxService>();

            // Register message processing strategy and serialization factory as singletons
            services.AddSingleton<IMessageProcessorStrategie, MessageProcessorStrategie>();
            services.AddSingleton<IModelSerializationFactory, ModelSerializationFactory>();
            services.AddSingleton<ModelJsonContext>(ModelJsonContext.Default);

            // Register hosted services for Kafka consumer and polling workers
            services.AddHostedService<KafkaConsumService>(); // Kafka consumer background service
            services.AddHostedService<KafkaPollingWorker>(); // Polling worker background service

            // Optional: Register KafkaConsumerService as Singleton or Transient for testing
            services.AddSingleton<KafkaConsumService>();  // Or services.AddTransient<KafkaConsumService>();
        }
    }
}
