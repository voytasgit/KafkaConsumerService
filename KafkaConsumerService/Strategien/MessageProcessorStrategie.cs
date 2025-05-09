using KafkaConsumerService.Factories;
using KafkaConsumerService.Models;
using KafkaConsumerService.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace KafkaConsumerService.Strategien
{
    public interface IMessageProcessorStrategie
    {
        Task ProcessAsync(string message);
    }
    public class MessageProcessorStrategie : IMessageProcessorStrategie
    {
        private readonly ILogger<MessageProcessorStrategie> _logger;
        private readonly IModelServiceFactory _modelServiceFactory;
        private readonly IModelSerializationFactory _modelSerializationFactory;

        public MessageProcessorStrategie(
            ILogger<MessageProcessorStrategie> logger,
            IModelServiceFactory modelServiceFactory,
            IModelSerializationFactory modelSerializationFactory)
        {
            _logger = logger;
            _modelServiceFactory = modelServiceFactory;
            _modelSerializationFactory = modelSerializationFactory;
        }

        public async Task ProcessAsync(string message)
        {
            // deserialisieren message, angekommen als Model AP_KAFKA_QUEUE in Json
            var evt = JsonSerializer.Deserialize<AP_KAFKA_QUEUE>(message, ModelJsonContext.Default.AP_KAFKA_QUEUE);
            if (evt == null)
            {
                _logger.LogDebug("Message not desertialised as Queue Table: " + message);
                return;
            }

            var dataJsonString = evt.DATA; // holen die Daten
            if (dataJsonString == null)
            {
                _logger.LogDebug("Message DATA are empty: " + message);
                return;
            }
            var action = evt.ACTION_NAME.ToLower(); // erkennen Aktion CRUID
            string modelName = evt.TABLE_NAME; // erkenne Model
            // prüfen ob Modell deserialisierbar ist
            var model = _modelSerializationFactory.DeserializeByModelName(modelName, dataJsonString);
            if (model == null)
            {
                _logger.LogDebug("Model not deserialised. Name: " + modelName + " - Message:" + message);
                return;
            }
            // hollen für das Model passendes CRUID Service
            var service = _modelServiceFactory.GetService(modelName);
            await service.HandleAsync(action, dataJsonString);
        }
    }
}
