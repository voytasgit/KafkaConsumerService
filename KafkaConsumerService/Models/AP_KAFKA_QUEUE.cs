using System;
using System.Collections.Generic;

namespace KafkaConsumerService.Models;
// Represents the AP_KAFKA_QUEUE model with various properties related to Kafka queue events.
public partial class AP_KAFKA_QUEUE
{
    public string QUEUE_ID { get; set; }

    public string TABLE_NAME { get; set; } = null!;

    public string ACTION_NAME { get; set; } = null!;

    public string KEY_VALUE { get; set; } = null!;

    public string? DATA { get; set; }

    public DateTime? EVENT_TIME { get; set; }

    public string? ProcessingStatus { get; set; }
}
