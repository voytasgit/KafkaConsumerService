using Confluent.Kafka;
using KafkaConsumerService.Configuration;
using KafkaConsumerService.Models;
using KafkaConsumerService.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace KafkaConsumerService.Services
{
    // Interface for Kafka producer service, defines the method to produce messages to Kafka.
    public interface IKafkaProducerService
    {
        Task ProduceAsync(AP_KAFKA_QUEUE entry); // Method to produce a message to Kafka
    }

    // KafkaProducerService is responsible for producing messages to a Kafka topic.
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;  // Kafka producer instance
        private readonly ILogger<KafkaProducerService> _logger; // Logger to log messages
        private readonly KafkaConfig _kafkaConfig; // Kafka configuration, holds the broker details, topic, etc.

        // Constructor initializes Kafka producer with configuration and logger.
        public KafkaProducerService(ILogger<KafkaProducerService> logger, KafkaConfig kafkaConfig) // Removed IOptions due to AOT compilation
        {
            _logger = logger;
            _kafkaConfig = kafkaConfig; // Using provided KafkaConfig instance directly for AOT compatibility
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,  // Kafka broker address
                MessageTimeoutMs = 5000,  // Timeout for producing a message (in milliseconds)
                Acks = Acks.All,  // Ensure that all brokers acknowledge the message
                Debug = "all" // Enable detailed debugging logs for the Kafka producer
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();  // Create and configure the Kafka producer
        }

        // Method to produce a message (AP_KAFKA_QUEUE entry) to Kafka.
        public async Task ProduceAsync(AP_KAFKA_QUEUE queueEntry)
        {
            try
            {
                // Serialize the entry into a JSON string
                string entry = JsonSerializer.Serialize<AP_KAFKA_QUEUE>(queueEntry, ModelJsonContext.Default.AP_KAFKA_QUEUE);

                // Create a Kafka message with the serialized entry
                var message = new Message<Null, string>
                {
                    Value = entry  // The message value is the serialized string
                };

                // Send the message to the Kafka topic
                var deliveryResult = await _producer.ProduceAsync(_kafkaConfig.Topic, message);

                // Log successful message delivery
                _logger.LogInformation($"Message successfully sent to Kafka: {deliveryResult.Value}");
            }
            catch (ProduceException<Null, string> ex)
            {
                // Log any errors that occur during message production
                _logger.LogError($"Error while producing message: {ex.Error.Reason}");
            }
            catch (Exception ex)
            {
                // Log general errors (e.g., network issues, serialization errors, etc.)
                _logger.LogError($"Error sending the message to Kafka: {ex.Message}");
            }
        }
    }
}
