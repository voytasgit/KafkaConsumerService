using Microsoft.VisualStudio.TestTools.UnitTesting;
using KafkaConsumerService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KafkaConsumerService.Models;
using KafkaConsumerService.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaConsumerService.Services.Tests
{
    [TestClass]
    public class KafkaConsumServiceTests
    {
        private IServiceProvider _serviceProvider;
        private IHost _host;
        private string _testTopic = "test-topic";
        private TaskCompletionSource<bool> _taskCompletionSource; // Zum Steuern der Fortsetzung der Verarbeitung

        [TestInitialize]
        public void Setup()
        {
            // Testkonfiguration laden
            var configuration = ConfigurationLoader.Load();

            // ServiceCollection für Dependency Injection einrichten
            var services = new ServiceCollection();
            ServiceRegistrator.RegisterServices(services, configuration);

            // KafkaConsumer-Host starten
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, serviceCollection) =>
                {
                    ServiceRegistrator.RegisterServices(serviceCollection, configuration);
                })
                .Build();

            _serviceProvider = _host.Services;
            _taskCompletionSource = new TaskCompletionSource<bool>(); // Initialisiere TaskCompletionSource
        }

        [TestMethod]
        public async Task KafkaConsumService_ShouldProcessMessageAndContinue()
        {
            // Arrange
            var kafkaProducerService = _serviceProvider.GetRequiredService<IKafkaProducerService>();

            // Nachricht, die an Kafka gesendet wird
            var testMessage = "Test Nachricht für KafkaConsumerService";

            // Testnachricht an Kafka senden
            // await kafkaProducerService.ProduceAsync(testMessage);

            // Starten des Kafka Consum Service im Hintergrund
            var kafkaConsumService = _serviceProvider.GetRequiredService<KafkaConsumService>();

            // Hintergrunddienst starten, aber wir wollen warten, bis die Verarbeitung abgeschlossen ist
            var task = kafkaConsumService.StartAsync(default);

            // Nun warten wir, bis der BackgroundService die Nachricht verarbeitet hat
            // Dies ist ein Block, der den Test verzögert, bis ProcessAsync abgeschlossen ist
            await _taskCompletionSource.Task;

            // Weitere Aktionen oder Verifikationen nach der Verarbeitung
            // Hier kannst du prüfen, ob bestimmte Zustände erreicht wurden, z. B. Logs oder Datenbankänderungen

            // Beispiel: Überprüfen, ob die Nachricht verarbeitet wurde und Logausgabe erzeugt wurde
            var logs = CaptureLogs(); // Funktion zum Erfassen der Logs
            Assert.IsTrue(logs.Contains("Nachricht verarbeitet"));

            // Test erfolgreich abgeschlossen
            Assert.IsTrue(true); // Placeholder, falls du noch andere Prüfungen hinzufügen möchtest
        }

        public void CompleteProcessing()
        {
            // Diese Methode wird im Hintergrund-Worker aufgerufen, um den Test fortzusetzen
            _taskCompletionSource.TrySetResult(true);
        }

        private string CaptureLogs()
        {
            // Implementiere das Erfassen der Logs, z. B. durch einen InMemoryLogger oder die Überprüfung der Ausgabe
            return string.Empty; // Platzhalter für das Log-Capturing
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Aufräumen nach dem Test
            _host?.Dispose();
        }
    }
}