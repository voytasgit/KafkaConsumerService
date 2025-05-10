using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaConsumerService.Models
{
    // Represents the AP_KAFKA_QUEUE model, including possible processing statuses.
    public partial class AP_KAFKA_QUEUE
    {
        public enum ProcStatus
        { 
            Sent,
            Pending,
            Error
        }
    }
}
