using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace InvoiceApp
{
    public static class CosmosDBChangeHandler
    {
        [FunctionName("CosmosDBChangeHandler")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "invoiceapp",
            collectionName: "invoiceapp",
            ConnectionStringSetting = "InvoicesDB_ConnectionString",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> input,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);

                var eventGridEventList = new List<EventGridEvent>();

                EventGridPublisherClient client = new EventGridPublisherClient(
                    new Uri("https://invoices.centralus-1.eventgrid.azure.net/api/events"),
                    new Azure.AzureKeyCredential("aD+FIgxoJ9+T+hi1fb7Biln+KLD0CXBz71q5ajyMl4g="));

                foreach (var document in input)
                {
                    var eventGridEvent = new EventGridEvent("InvoiceItem", "Invoice.Changed", "1.0", document);
                    eventGridEventList.Add(eventGridEvent);
                }
                await client.SendEventsAsync(eventGridEventList);
            }
        }
    }
}
