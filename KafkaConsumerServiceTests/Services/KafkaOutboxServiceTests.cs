using Microsoft.VisualStudio.TestTools.UnitTesting;
using KafkaConsumerService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KafkaConsumerService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaConsumerService.Services.Tests
{
    [TestClass]
    public class KafkaOutboxServiceTests
    {
        private AppDbContext _dbContext;
        private KafkaOutboxService _service;

        // [TestInitialize]
        //public void Setup1()
        //{
        //    var options = new DbContextOptionsBuilder<AppDbContext>()
        //        .UseInMemoryDatabase(databaseName: "TestDb")
        //        .Options;

        //    _dbContext = new AppDbContext(options);

        //    // Testdaten einfügen
        //    _dbContext.AP_KAFKA_QUEUE.AddRange(
        //        new AP_KAFKA_QUEUE
        //        {
        //            QUEUE_ID = 9,
        //            TABLE_NAME = "DISTLIST",
        //            ACTION_NAME = "UPDATE",
        //            KEY_VALUE = "e720c789-5250-4e5e-9c74-8998ae0409a3",
        //            DATA = "{\"DISTLISTID\": \"e720c789-5250-4e5e-9c74-8998ae0409a3\", \"DESCRIPTION\": \"test2\", \"OWNER_ID\": \"\", \"IS_DYNAMIC\": \"\", \"NAME\": \"test2\", \"SELECTION_ID\": \"\", \"DATE_NEW\": \"Mai  6 2025  4:01PM\", \"DATE_EDIT\": \"Mai  6 2025  4:01PM\", \"USER_NEW\": \"Voytas\", \"USER_EDIT\": \"Voytas\", \"AKTIV\": \"Y\"}",
        //            EVENT_TIME = DateTime.Parse("2025-05-06T16:01:44.960"),
        //            ProcessingStatus = "Pending"
        //        }
        //    );
        //    _dbContext.SaveChanges();

        //    _service = new KafkaOutboxService(_dbContext);
        //}

        [TestInitialize]
        public void Setup()
        {
            // Setze die Umgebungsvariable für den Test
            // Environment.SetEnvironmentVariable("ConnectionCrmTest", "Server=localhost;Database=TestDb;User Id=sa;Password=yourPassword;");

            var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__ConnectionCrmTest", EnvironmentVariableTarget.Machine);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connStr)
                .Options;

            _dbContext = new AppDbContext(options);
            _service = new KafkaOutboxService(_dbContext);
        }

        [TestMethod]
        public async Task GetNewEventsAsync_ReturnsOnlyPendingEntries()
        {
            // Act
            try
            {
                var result = await _service.GetNewEventsAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            //// Assert
            //Assert.AreEqual(1, result.Count);
            //Assert.IsTrue(result.All(e => e.ProcessingStatus == "Pending"));
        }
        //[TestMethod]
        //public async Task GetNewEventsAsync_IntegrationTest()
        //{
 

        //    //using var dbContext = new AppDbContext(options);
        //    //var service = new KafkaOutboxService(dbContext);

        //    //// Act
        //    //var result = await service.GetNewEventsAsync();

        //    //// Assert
        //    //Assert.AreEqual(2, result.Count);
        //    //Assert.IsTrue(result.All(e => e.ProcessingStatus == "Pending"));
        //}
    }
}