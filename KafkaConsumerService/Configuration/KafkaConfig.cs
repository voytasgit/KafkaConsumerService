using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaConsumerService.Configuration
{
    public class KafkaConfig
    {
        public string BootstrapServers { get; set; } = "";
        public string Topic { get; set; } = "";
        public string ModelNameSpace { get; set; } = "";
        public string GroupId { get; set; } = "";
    }

}
