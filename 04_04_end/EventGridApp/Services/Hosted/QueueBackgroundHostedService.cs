
using Azure.Messaging.EventGrid;

namespace EventGridApp.Services.Hosted
{
  
    public class QueueBackgroundHostedService : IHostedService, IDisposable
    {
      

        private Timer _timer = null!;
        QueueProcessor _queueProcessor;
        public QueueBackgroundHostedService(IConfiguration config)
        {
            _queueProcessor = new QueueProcessor(config.GetConnectionString("StorageConnectionString"), "invoices");
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            var messages = await _queueProcessor.GetMessages();
            if (messages.Any())
            {
                foreach(var message in messages)
                {
                    var data = message.Body.ToObjectFromJson<EventGridEvent>();
                 
                    //delete messages after processing
                    await _queueProcessor.RemoveMessage(message.MessageId, message.PopReceipt);
                }
            }

           
           

        }

        public Task StopAsync(CancellationToken stoppingToken)
        {


            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

     
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
