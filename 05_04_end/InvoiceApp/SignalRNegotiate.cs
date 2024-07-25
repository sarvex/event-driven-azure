﻿using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace InvoiceApp
{
    public static class SignalRNegotiate
    {
        private static HttpClient httpClient = new HttpClient();
        private static string Etag = string.Empty;
        private static string StarCount = "0";

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "Invoices")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
        [FunctionName("broadcast")]
        public static async Task Broadcast([EventGridTrigger] EventGridEvent eventGridEvent,
        [SignalR(HubName = "Invoices")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
          

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { eventGridEvent }
                });
        }
       

    }
}