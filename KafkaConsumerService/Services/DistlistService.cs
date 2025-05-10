using KafkaConsumerService.Models;
using KafkaConsumerService.Repository;
using KafkaConsumerService.Serialization;
using Serilog;
using System.Text.Json;

namespace KafkaConsumerService.Services
{
    // Service to handle actions related to DISTLIST entities (create, update, delete).
    public interface IDistlistService
    {
        // Handles an action on a DISTLIST entity, such as create, update, or delete.
        Task HandleAsync(string action, string jsonData);
    }

    // Implementation of the DISTLIST service, responsible for processing actions on DISTLIST entities.
    public class DistlistService : IDistlistService
    {
        private readonly IGenericRepository<DISTLIST> _repository;

        public DistlistService(IGenericRepository<DISTLIST> repository)
        {
            _repository = repository;
        }

        // Deserialize the data, log errors, and perform the specified action (create, update, delete).
        public async Task HandleAsync(string action, string dataJsonString)
        {
            // Deserialize the JSON string into a DISTLIST model.
            var model = JsonSerializer.Deserialize<DISTLIST>(dataJsonString, ModelJsonContext.Default.DISTLIST);
            if (model == null)
            {
                Log.Debug("DISTLIST can not be deserialized: " + dataJsonString);
                return;
            }

            // Perform the specified action on the model.
            switch (action.ToLower())
            {
                case "create":
                    await _repository.CreateAsync(model);
                    break;
                case "update":
                    await _repository.UpdateAsync(model);
                    break;
                case "delete":
                    await _repository.DeleteAsync(model);
                    break;
                default:
                    Log.Debug("Unsupported action: " + action);
                    break;
            }
        }
    }
}
