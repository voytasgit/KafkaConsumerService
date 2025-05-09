using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using KafkaConsumerService.Models;

namespace KafkaConsumerService.Services.Tests
{
    [TestClass()]
    public class EventPollingServiceTests
    {
        private readonly Mock<IKafkaOutboxService> mockEventQueueService;
        private readonly Mock<IKafkaProducerService> mockKafkaProducerService;
        private readonly Mock<ILogger<KafkaPollingWorker>> mockLogger;
        private readonly KafkaPollingWorker eventPollingService;

        public EventPollingServiceTests()
        {
            mockEventQueueService = new Mock<IKafkaOutboxService>();
            mockKafkaProducerService = new Mock<IKafkaProducerService>();
            mockLogger = new Mock<ILogger<KafkaPollingWorker>>();

            eventPollingService = new KafkaPollingWorker(
                Mock.Of<ILogger<KafkaPollingWorker>>(),
                mockKafkaProducerService.Object,
                mockEventQueueService.Object
            );

            eventPollingService = new KafkaPollingWorker(
                mockLogger.Object,
                mockKafkaProducerService.Object,
                mockEventQueueService.Object
            );

        }

        [Fact]
        public async Task PollForNewEntries_ShouldCallKafkaProducerForEachEvent()
        {

            var events = new List<AP_KAFKA_QUEUE>
            {
                new AP_KAFKA_QUEUE
                {
                    QUEUE_ID = "A720c789-5250-4e5e-9c74-8998ae0409a4",
                    TABLE_NAME = "DISTLIST",
                    ACTION_NAME = "UPDATE",
                    KEY_VALUE = "e720c789-5250-4e5e-9c74-8998ae0409a3",
                    DATA = "{\"DISTLISTID\": \"e720c789-5250-4e5e-9c74-8998ae0409a3\", \"DESCRIPTION\": \"test2\", \"OWNER_ID\": \"\", \"IS_DYNAMIC\": \"\", \"NAME\": \"test2\", \"SELECTION_ID\": \"\", \"DATE_NEW\": \"Mai  6 2025  4:01PM\", \"DATE_EDIT\": \"Mai  6 2025  4:01PM\", \"USER_NEW\": \"Voytas\", \"USER_EDIT\": \"Voytas\", \"AKTIV\": \"Y\"}",
                    EVENT_TIME = DateTime.Parse("2025-05-06T16:01:44.960"),
                    ProcessingStatus = "Pending"
                }
            };


            mockEventQueueService.Setup(s => s.GetNewEventsAsync()).ReturnsAsync(events);

            // Act
            //await eventPollingService.PollForNewEntries();

            // Assert
            mockEventQueueService.Verify(s => s.GetNewEventsAsync(), Times.Once);
            mockKafkaProducerService.Verify(k => k.ProduceAsync(It.IsAny<AP_KAFKA_QUEUE>()), Times.Once);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Polling nach neuen Events startet")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }
    }
}