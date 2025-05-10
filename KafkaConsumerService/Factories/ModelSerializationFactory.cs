using KafkaConsumerService.Models;
using KafkaConsumerService.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KafkaConsumerService.Factories
{
    // Factory class for deserializing models based on their name
    public interface IModelSerializationFactory
    {
        object? DeserializeByModelName(string modelName, string json);
    }
    public class ModelSerializationFactory : IModelSerializationFactory
    {

        public object? DeserializeByModelName(string modelName, string json)
        {
            return modelName.ToUpperInvariant() switch
            {
                "DISTLIST" => JsonSerializer.Deserialize<DISTLIST>(json, ModelJsonContext.Default.DISTLIST),
                "AP_KAFKA_QUEUE" => JsonSerializer.Deserialize<AP_KAFKA_QUEUE>(json, ModelJsonContext.Default.AP_KAFKA_QUEUE),
                _ => throw new NotSupportedException($"Deserialization for model '{modelName}' is not supported.")
            };
        }
    }
}
