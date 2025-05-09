using KafkaConsumerService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaConsumerService.Factories
{
    public interface IModelServiceFactory
    {
        IDistlistService GetService(string modelName);
    }

    public class ModelServiceFactory : IModelServiceFactory
    {
        private readonly IServiceProvider _provider;

        public ModelServiceFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IDistlistService GetService(string modelName)
        {
            return modelName.ToUpper() switch
            {
                "DISTLIST" => _provider.GetRequiredService<IDistlistService>(),
                // weitere Modelle hier
                _ => throw new NotSupportedException($"Unbekanntes Modell: {modelName}")
            };
        }
    }
}
