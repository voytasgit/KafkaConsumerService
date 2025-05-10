using KafkaConsumerService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaConsumerService.Services
{
    // Service for managing the outbox for Kafka events. 
    // It retrieves new events and updates their processing status.
    public interface IKafkaOutboxService
    {
        // Retrieves a list of new events (with status "Pending").
        Task<List<AP_KAFKA_QUEUE>> GetNewEventsAsync();

        // Updates the status of a specific event.
        Task UpdateEventStatusAsync(string eventId, string newStatus);
    }

    // Implementation of the KafkaOutboxService, responsible for interacting with the database
    // to get new events and update their status.
    public class KafkaOutboxService : IKafkaOutboxService
    {
        private readonly AppDbContext _dbContext;

        // Constructor to inject the DbContext dependency for database access.
        public KafkaOutboxService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Retrieves new events with the status "Pending" from the database.
        public async Task<List<AP_KAFKA_QUEUE>> GetNewEventsAsync()
        {
            return await _dbContext.AP_KAFKA_QUEUE
                .Where(x => x.ProcessingStatus == Enum.GetName(typeof(AP_KAFKA_QUEUE.ProcStatus), AP_KAFKA_QUEUE.ProcStatus.Pending)) // Example status "New" = Pending
                .OrderBy(x => x.EVENT_TIME)  // Orders the events by their event time.
                .ToListAsync();  // Executes the query and retrieves the list asynchronously.
        }

        // Updates the processing status of a specific event in the database.
        public async Task UpdateEventStatusAsync(string eventId, string newStatus)
        {
            var eventEntry = await _dbContext.AP_KAFKA_QUEUE.FindAsync(eventId);  // Finds the event by its ID.
            if (eventEntry != null)
            {
                eventEntry.ProcessingStatus = newStatus;  // Updates the status of the event.
                await _dbContext.SaveChangesAsync();  // Saves the changes asynchronously to the database.
            }
        }
    }
}
