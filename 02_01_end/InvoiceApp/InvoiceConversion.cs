// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Documents;
using Invoices.Data.Services;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid.SystemEvents;
using System.Linq;
using System.IO;

namespace InvoiceApp
{
    public static class InvoiceConversion
    {
        [FunctionName("InvoiceConversion")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
            var storageConnectionString = Environment.GetEnvironmentVariable("InvoiceStorage_ConnectionString");
           
            if (eventGridEvent.EventType == SystemEventNames.StorageBlobCreated)
            {
                var eventData = eventGridEvent.Data.ToObjectFromJson<StorageBlobCreatedEventData>();

                var fileName = eventData.Url.Split('/').LastOrDefault();
                if (string.IsNullOrEmpty(fileName) || !Path.GetExtension(fileName).Equals(".docx", StringComparison.InvariantCultureIgnoreCase)) return;
                else
                {
                    var invoiceFileService = new InvoiceFileService(new Invoices.Data.Models.Config.StorageConfig
                    {
                        ConnectionString = storageConnectionString,
                        ContainerName = "invoices"
                    });
                    var invoiceIdFromFileName = Path.GetFileNameWithoutExtension(fileName);
                    await invoiceFileService.ConvertInvoiceToAllFormats(invoiceIdFromFileName);
                }
               
            }
        }
    }
}
