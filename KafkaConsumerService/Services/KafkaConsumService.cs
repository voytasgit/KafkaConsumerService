using Confluent.Kafka;
using KafkaConsumerService.Configuration;
using KafkaConsumerService.Factories;
using KafkaConsumerService.Strategien;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaConsumerService.Services
{
    public class KafkaConsumService : BackgroundService
    {
        private readonly ILogger<KafkaConsumService> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly KafkaConfig _kafkaConfig;
        private readonly IMessageProcessorStrategie _messageProcessorStrategie;
        public KafkaConsumService(ILogger<KafkaConsumService> logger
            , KafkaConfig kafkaConfig
            , IModelServiceFactory modelServiceFactory
            , IModelSerializationFactory modelSerializationFactory
            , IMessageProcessorStrategie messageProcessorStrategie)
        {
            _logger = logger;
            _kafkaConfig = kafkaConfig;  // Zugriff auf die Konfiguration
            _messageProcessorStrategie = messageProcessorStrategie;
            // Kafka Consumer Konfiguration
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,  // Kafka Broker Adresse
                GroupId = _kafkaConfig.GroupId,     // Consumer-Gruppe
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            // Kafka-Consumer erstellen
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }
        // Dies ist die Methode, die den Hintergrunddienst startet
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_kafkaConfig.Topic);
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(200));

                    if (consumeResult != null)
                    {
                        _logger.LogInformation($"Nachricht empfangen: {consumeResult.Message.Value}");
                        await _messageProcessorStrategie.ProcessAsync(consumeResult.Message.Value);
                    }

                    await Task.Delay(100, cancellationToken); // optional: etwas Pause zwischen Polls
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer wurde abgebrochen.");
            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}
