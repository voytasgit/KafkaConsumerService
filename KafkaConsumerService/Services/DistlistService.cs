using KafkaConsumerService.Models;
using KafkaConsumerService.Repository;
using KafkaConsumerService.Serialization;
using Serilog;
using System.Text.Json;

namespace KafkaConsumerService.Services
{
    public interface IDistlistService
    {
        Task HandleAsync(string action, string jsonData);
    }
    public class DistlistService : IDistlistService
    {
        private readonly IGenericRepository<DISTLIST> _repository;

        public DistlistService(IGenericRepository<DISTLIST> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(string action, string dataJsonString)
        {
            var model = JsonSerializer.Deserialize<DISTLIST>(dataJsonString, ModelJsonContext.Default.DISTLIST);
            if (model == null)
            {
                Log.Debug("DISTLIST can not be desertialised : " + dataJsonString);
                return;
            }
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
            }
        }
    }
}
