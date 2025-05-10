using KafkaConsumerService.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaConsumerService.Services
{
    // Kafka polling worker service that periodically polls for new events
    // and sends them to Kafka using the producer service.
    public class KafkaPollingWorker : BackgroundService
    {
        private readonly ILogger<KafkaPollingWorker> _logger;
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly IKafkaOutboxService _kafkaOutboxService;

        // Constructor to initialize the necessary dependencies (logger, producer service, outbox service).
        public KafkaPollingWorker(
            ILogger<KafkaPollingWorker> logger,
            IKafkaProducerService kafkaProducerService,
            IKafkaOutboxService kafkaOutboxService)
        {
            _logger = logger;
            _kafkaProducerService = kafkaProducerService;
            _kafkaOutboxService = kafkaOutboxService;
        }

        // Executes the background task of polling and producing messages asynchronously.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("KafkaPollingWorker started.");

            // Keep running the polling loop until the cancellation token is triggered
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting to poll for new events...");

                    // Get new events from the outbox service
                    var newEntries = await _kafkaOutboxService.GetNewEventsAsync();

                    // Iterate through each new event and send it to Kafka
                    foreach (var entry in newEntries)
                    {
                        // Produce the event to Kafka
                        await _kafkaProducerService.ProduceAsync(entry);

                        // Update the event status to "Sent"
                        await _kafkaOutboxService.UpdateEventStatusAsync(entry.QUEUE_ID, AP_KAFKA_QUEUE.ProcStatus.Sent.ToString());
                    }
                }
                catch (OperationCanceledException)
                {
                    // Gracefully handle cancellation of the polling process
                    _logger.LogInformation("Polling Worker has been cancelled.");
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur during polling or producing
                    _logger.LogError(ex, "Error during polling/producing");
                }

                // Wait for a specified time (e.g., 5 minutes) before polling again
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("KafkaPollingWorker is stopping.");
        }
    }
}
