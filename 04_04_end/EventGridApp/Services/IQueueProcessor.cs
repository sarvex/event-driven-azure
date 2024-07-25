using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace EventGridApp.Services
{
    public class QueueProcessor
    {
        private readonly QueueClient _queue;
        public QueueProcessor(string connectionString,string queueName)
        {
            _queue= new QueueClient(connectionString, queueName, new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
        }

        public async Task<IEnumerable<QueueMessage>> GetMessages()
        {
            var messages = await _queue.ReceiveMessagesAsync(30, TimeSpan.FromSeconds(60));
            return messages.Value;
        }
        public async Task RemoveMessage(string id, string popReceipt)
        {
            await _queue.DeleteMessageAsync(id, popReceipt);
        }
    }
}
