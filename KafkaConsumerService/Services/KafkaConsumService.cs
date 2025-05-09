using Confluent.Kafka;
using KafkaConsumerService.Configuration;
using KafkaConsumerService.Factories;
using KafkaConsumerService.Models;
using KafkaConsumerService.Serialization;
using KafkaConsumerService.Strategien;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text.Json;

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
            _consumer.Subscribe(_kafkaConfig.Topic); // Kafka-Topic abonnieren

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Warten auf neue Nachrichten
                    var consumeResult = _consumer.Consume(cancellationToken);

                    // Verarbeiten der Nachricht
                    _logger.LogInformation($"Nachricht empfangen: {consumeResult.Message.Value}");

                    var message = consumeResult.Message.Value;
                    // Nachricht verarbeiten
                    await _messageProcessorStrategie.ProcessAsync(message);
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully
                _logger.LogInformation("Consumer wurde abgebrochen.");
            }
            catch (ConsumeException e)
            {
                _logger.LogError($"Fehler beim Konsumieren der Nachricht: {e.Error.Reason}");
            }
            finally
            {
                _consumer.Close();
            }
        }



    }
}
