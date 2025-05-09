using KafkaConsumerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KafkaConsumerService.Utils;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KafkaConsumerService.Services.Tests
{
    [TestClass()]
    public class KafkaProducerServiceTests
    {
        private ServiceProvider _serviceProvider;
        private AppDbContext _dbContext;

        [TestInitialize]
        public void Setup()
        {
            var configuration = ConfigurationLoader.Load(); // z. B. für Test eigene appsettings.Test.json nutzen

            var services = new ServiceCollection();
            ServiceRegistrator.RegisterServices(services, configuration);

            // Alle bisherigen AppDbContext-Registrierungen entfernen
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            //// Wenn du bestimmte Services austauschen willst (z. B. echte Producer durch Mocks)
            //services.RemoveAll<IProducer<Null, string>>();
            //services.AddSingleton(Mock.Of<IProducer<Null, string>>());

            // ggf. Mocking, DbContext überschreiben usw.
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TestDb"));

            _serviceProvider = services.BuildServiceProvider();


            // Seed Testdaten
            _dbContext = _serviceProvider.GetRequiredService<AppDbContext>();

            // Testdaten einfügen
            _dbContext.AP_KAFKA_QUEUE.AddRange(
                new AP_KAFKA_QUEUE
                {
                    QUEUE_ID = "A720c789-5250-4e5e-9c74-8998ae0409a1",
                    TABLE_NAME = "DISTLIST",
                    ACTION_NAME = "UPDATE",
                    KEY_VALUE = "e720c789-5250-4e5e-9c74-8998ae0409a3",
                    DATA = @"{""QUEUE_ID"":""A720c789-5250-4e5e-9c74-8998ae0409a0"",""TABLE_NAME"":""DISTLIST"",""ACTION_NAME"":""UPDATE"",""KEY_VALUE"":""e720c789-5250-4e5e-9c74-8998ae0409a3"",""DATA"":""{\u0022DISTLISTID\u0022: \u0022e720c789-5250-4e5e-9c74-8998ae0409a3\u0022, \u0022DESCRIPTION\u0022: \u0022test2\u0022, \u0022OWNER_ID\u0022: null, \u0022IS_DYNAMIC\u0022: \u0022\u0022, \u0022NAME\u0022: \u0022test2\u0022, \u0022SELECTION_ID\u0022: \u0022\u0022, \u0022DATE_NEW\u0022: \u00222024-11-07T14:56:04\u0022, \u0022DATE_EDIT\u0022: \u00222024-11-07T14:56:04\u0022, \u0022USER_NEW\u0022: \u0022Voytas\u0022, \u0022USER_EDIT\u0022: \u0022Voytas\u0022, \u0022AKTIV\u0022: \u0022Y\u0022}"",""EVENT_TIME"":""2025-05-06T16:01:44.96"",""ProcessingStatus"":""Pending""}",
                    EVENT_TIME = DateTime.Parse("2025-05-06T16:01:44.960"),
                    ProcessingStatus = "Pending"
                }
            );
            _dbContext.SaveChanges();
        }
        [TestMethod()]
        public async Task ProduceAsyncTest()
        {
            var service = _serviceProvider.GetRequiredService<IKafkaProducerService>();
            var queueEntry = await _dbContext.AP_KAFKA_QUEUE.FirstOrDefaultAsync();
            await service.ProduceAsync(queueEntry);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
        }
    }
}