using KafkaConsumerService.Factories;
using KafkaConsumerService.Models;
using KafkaConsumerService.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace KafkaConsumerService.Strategien
{
    // Interface defining the message processing strategy, used to process messages asynchronously.
    public interface IMessageProcessorStrategie
    {
        Task ProcessAsync(string message); // Method to process a message
    }

    // Implementation of the IMessageProcessorStrategie interface.
    public class MessageProcessorStrategie : IMessageProcessorStrategie
    {
        private readonly ILogger<MessageProcessorStrategie> _logger; // Logger to log processing details and errors
        private readonly IModelServiceFactory _modelServiceFactory; // Factory to get the correct service for each model
        private readonly IModelSerializationFactory _modelSerializationFactory; // Factory to deserialize the data for each model

        // Constructor to initialize logger, model service factory, and model serialization factory.
        public MessageProcessorStrategie(
            ILogger<MessageProcessorStrategie> logger,
            IModelServiceFactory modelServiceFactory,
            IModelSerializationFactory modelSerializationFactory)
        {
            _logger = logger;
            _modelServiceFactory = modelServiceFactory;
            _modelSerializationFactory = modelSerializationFactory;
        }

        // Method to process the incoming message.
        public async Task ProcessAsync(string message)
        {
            // Deserialize the incoming message, expecting it to be an AP_KAFKA_QUEUE model in JSON format.
            var evt = JsonSerializer.Deserialize<AP_KAFKA_QUEUE>(message, ModelJsonContext.Default.AP_KAFKA_QUEUE);
            if (evt == null)
            {
                _logger.LogDebug("Message not deserialized as Queue Table: " + message);
                return; // If deserialization fails, log and return
            }

            var dataJsonString = evt.DATA; // Get the data field from the event
            if (dataJsonString == null)
            {
                _logger.LogDebug("Message DATA is empty: " + message);
                return; // If the data field is empty, log and return
            }

            var action = evt.ACTION_NAME.ToLower(); // Identify the action type (e.g., create, update, delete)
            string modelName = evt.TABLE_NAME; // Get the model name (table name)

            // Check if the model can be deserialized using the model name.
            var model = _modelSerializationFactory.DeserializeByModelName(modelName, dataJsonString);
            if (model == null)
            {
                _logger.LogDebug("Model not deserialized. Name: " + modelName + " - Message: " + message);
                return; // If deserialization fails for the model, log and return
            }

            // Retrieve the appropriate CRUD service for the model.
            var service = _modelServiceFactory.GetService(modelName);
            await service.HandleAsync(action, dataJsonString); // Process the action using the appropriate service
        }
    }
}
