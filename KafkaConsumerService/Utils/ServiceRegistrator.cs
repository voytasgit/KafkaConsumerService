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
    public static class ServiceRegistrator
    {

        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // appsettings.json lesen
            var useInMemory = string.Equals(configuration["UseInMemoryDb"] ?? "false", "true", StringComparison.OrdinalIgnoreCase); // = configuration.GetValue<bool>("UseInMemoryDb");  // stattdessen AOT sicher
            // alle Tests bekommen InMemory DB
            if (useInMemory)
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            }
            else
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(configuration["ConnectionStrings:ConnectionCrmTest"])); // AOT // ConnectionStrings__ConnectionCrmTest
                                                                                                 //options.UseSqlServer(configuration.GetConnectionString("ConnectionCrmTest")));
            }

            //services.Configure<KafkaConfig>(configuration.GetSection("KafkaConfig")); // stattdessen AOT sicher
            var section = configuration.GetRequiredSection("KafkaConfig");
            var kafkaConfig = new KafkaConfig
            {
                BootstrapServers = section["BootstrapServers"] ?? throw new InvalidOperationException("KafkaConfig:BootstrapServers is missing or empty."),
                Topic = section["Topic"] ?? throw new InvalidOperationException("KafkaConfig:Topic is missing."),
                ModelNameSpace = section["ModelNameSpace"] ?? throw new InvalidOperationException("ModelNameSpace is missing."),
                GroupId = section["GroupId"] ?? throw new InvalidOperationException("KafkaConfig:GroupId is missing."),
            };
            services.AddSingleton(kafkaConfig);

            services.AddLogging(builder => builder.AddSerilog());

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IDistlistService, DistlistService>();
            services.AddScoped<IModelServiceFactory, ModelServiceFactory>();

            services.AddTransient<IKafkaProducerService, KafkaProducerService>();
            services.AddTransient<IKafkaOutboxService, KafkaOutboxService>();

            services.AddSingleton<IMessageProcessorStrategie, MessageProcessorStrategie>();
            services.AddSingleton<IModelSerializationFactory, ModelSerializationFactory>();
            services.AddSingleton<ModelJsonContext>(ModelJsonContext.Default);

            // Optional: Nur im Produktivcode
            services.AddHostedService<KafkaConsumService>();
            services.AddHostedService<KafkaPollingWorker>();

            // Registrierung als Singleton oder Transient fürs TEsten
            services.AddSingleton<KafkaConsumService>();  // Oder services.AddTransient<KafkaConsumService>();
        }
    }
}

