using KafkaConsumerService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaConsumerService.Services
{
    public interface IKafkaOutboxService
    {
        Task<List<AP_KAFKA_QUEUE>> GetNewEventsAsync();
        Task UpdateEventStatusAsync(string eventId, string newStatus);
    }
    public class KafkaOutboxService : IKafkaOutboxService
    {
        private readonly AppDbContext _dbContext;

        public KafkaOutboxService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AP_KAFKA_QUEUE>> GetNewEventsAsync()
        {
            return await _dbContext.AP_KAFKA_QUEUE
                .Where(x => x.ProcessingStatus == Enum.GetName(typeof(AP_KAFKA_QUEUE.ProcStatus), AP_KAFKA_QUEUE.ProcStatus.Pending)) // Beispielstatus "New" = Pending // (ProcStatus)Enum.Parse(typeof(ProcStatus), newStatus)
                .OrderBy(x=>x.EVENT_TIME)
                .ToListAsync();
        }

        public async Task UpdateEventStatusAsync(string eventId, string newStatus)
        {
            var eventEntry = await _dbContext.AP_KAFKA_QUEUE.FindAsync(eventId);
            if (eventEntry != null)
            {
                eventEntry.ProcessingStatus = newStatus;
                await _dbContext.SaveChangesAsync();
            }
        }
    }

}
