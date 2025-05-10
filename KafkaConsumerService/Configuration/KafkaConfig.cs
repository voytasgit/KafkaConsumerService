using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaConsumerService.Configuration
{
    /// <summary>
    /// Configuration settings for Kafka Consumer, including connection details.
    /// </summary>
    public class KafkaConfig
    {
        public string BootstrapServers { get; set; } = "";
        public string Topic { get; set; } = "";
        public string ModelNameSpace { get; set; } = "";
        public string GroupId { get; set; } = "";
    }

}
