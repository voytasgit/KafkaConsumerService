using Confluent.Kafka;
using KafkaConsumerService.Configuration;
using KafkaConsumerService.Models;
using KafkaConsumerService.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace KafkaConsumerService.Services
{
    public interface IKafkaProducerService
    {
        Task ProduceAsync(AP_KAFKA_QUEUE entry);
    }

    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly KafkaConfig _kafkaConfig;
        public KafkaProducerService(ILogger<KafkaProducerService> logger, KafkaConfig kafkaConfig) // , IOptions<KafkaConfig> kafkaConfig) wg. AOT
        {
            _logger = logger;
            _kafkaConfig = kafkaConfig; // wg. AOT .Value;  // Zugriff auf die Konfiguration
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,  // Kafka Broker Adresse
                MessageTimeoutMs = 5000,  // Timeout nach 5 Sekunden
                Acks = Acks.All,
                Debug = "all" // Aktiviert detaillierte Debugging-Logs
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProduceAsync(AP_KAFKA_QUEUE queueEntry)
        {
            try
            {
                string entry = JsonSerializer.Serialize<AP_KAFKA_QUEUE>(queueEntry, ModelJsonContext.Default.AP_KAFKA_QUEUE);
                var message = new Message<Null, string>
                {
                    Value = entry
                };

                var deliveryResult = await _producer.ProduceAsync(_kafkaConfig.Topic, message);

                _logger.LogInformation($"Nachricht erfolgreich an Kafka gesendet: {deliveryResult.Value}");
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError($"Fehler beim Produzieren: {ex.Error.Reason}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fehler beim Senden der Nachricht an Kafka: {ex.Message}");
            }
        }
    }
}
