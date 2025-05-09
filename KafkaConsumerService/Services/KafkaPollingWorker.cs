using KafkaConsumerService.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaConsumerService.Services
{
    public class KafkaPollingWorker : BackgroundService
    {
        private readonly ILogger<KafkaPollingWorker> _logger;
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly IKafkaOutboxService _kafkaOutboxService;

        public KafkaPollingWorker(
            ILogger<KafkaPollingWorker> logger,
            IKafkaProducerService kafkaProducerService,
            IKafkaOutboxService kafkaOutboxService)
        {
            _logger = logger;
            _kafkaProducerService = kafkaProducerService;
            _kafkaOutboxService = kafkaOutboxService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("KafkaPollingWorker gestartet.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starte Polling nach neuen Events...");
                    var newEntries = await _kafkaOutboxService.GetNewEventsAsync();

                    foreach (var entry in newEntries)
                    {
                        await _kafkaProducerService.ProduceAsync(entry);
                        await _kafkaOutboxService.UpdateEventStatusAsync(entry.QUEUE_ID, AP_KAFKA_QUEUE.ProcStatus.Sent.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fehler beim Polling/Producing");
                }

                // Wartezeit (z. B. 5 Minuten)
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("KafkaPollingWorker wird beendet.");
        }
    }
}
