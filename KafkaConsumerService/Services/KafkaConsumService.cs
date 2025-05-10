using Confluent.Kafka;
using KafkaConsumerService.Configuration;
using KafkaConsumerService.Factories;
using KafkaConsumerService.Strategien;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaConsumerService.Services
{
    // Kafka Consumer Service that consumes messages from Kafka and processes them using a processing strategy.
    public class KafkaConsumService : BackgroundService
    {
        private readonly ILogger<KafkaConsumService> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly KafkaConfig _kafkaConfig;
        private readonly IMessageProcessorStrategie _messageProcessorStrategie;

        // Constructor to initialize the Kafka consumer and dependency injection.
        public KafkaConsumService(ILogger<KafkaConsumService> logger
            , KafkaConfig kafkaConfig
            , IModelServiceFactory modelServiceFactory
            , IModelSerializationFactory modelSerializationFactory
            , IMessageProcessorStrategie messageProcessorStrategie)
        {
            _logger = logger;
            _kafkaConfig = kafkaConfig;  // Access to Kafka configuration
            _messageProcessorStrategie = messageProcessorStrategie;

            // Kafka consumer configuration
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,  // Kafka broker address
                GroupId = _kafkaConfig.GroupId,     // Consumer group
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            // Creating the Kafka consumer
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        // This method is used by the background service to consume messages and process them.
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_kafkaConfig.Topic); // Subscribes to the Kafka topic
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Try to consume a message
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(200));

                    // If a message is received, process it
                    if (consumeResult != null)
                    {
                        _logger.LogInformation($"Message received: {consumeResult.Message.Value}");
                        await _messageProcessorStrategie.ProcessAsync(consumeResult.Message.Value);  // Process the message
                    }

                    // Optional: short delay to control the polling interval
                    await Task.Delay(100, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer has been cancelled.");
            }
            finally
            {
                // Properly close the Kafka consumer
                _consumer.Close();
            }
        }
    }
}
