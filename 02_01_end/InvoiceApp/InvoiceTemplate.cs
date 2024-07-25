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

namespace InvoiceApp
{
    public static class InvoiceTemplate
    {
        [FunctionName("InvoiceTemplate")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
            var storageConnectionString = Environment.GetEnvironmentVariable("InvoiceStorage_ConnectionString");
            var cosmosDBConnectionString = Environment.GetEnvironmentVariable("InvoicesDB_ConnectionString");
            
            if (eventGridEvent.EventType == "Invoice.Changed")
            {
                var eventData = eventGridEvent.Data.ToObjectFromJson<Document>();


                var dataService = new DataService(new Invoices.Data.Models.Config.DbConfig
                {
                    ConnectionString = cosmosDBConnectionString,
                    ContainerId = "invoiceapp",
                    DatabaseId = "invoiceapp"
                });

                var invoiceDetails = await dataService.GetInvoiceDetails(eventData.Id);

                var invoiceFileService = new InvoiceFileService(new Invoices.Data.Models.Config.StorageConfig
                {
                    ConnectionString = storageConnectionString,
                    ContainerName = "invoices"
                });
                await invoiceFileService.GenerateInvoice(invoiceDetails);
            }
        }
    }
}
