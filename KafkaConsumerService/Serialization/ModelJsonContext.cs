﻿using KafkaConsumerService.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KafkaConsumerService.Serialization
{
    // Provides JSON serialization context for specific models (DISTLIST and AP_KAFKA_QUEUE).
    [JsonSerializable(typeof(DISTLIST))]
    [JsonSerializable(typeof(AP_KAFKA_QUEUE))]
    public partial class ModelJsonContext : JsonSerializerContext
    {
        public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }
}
