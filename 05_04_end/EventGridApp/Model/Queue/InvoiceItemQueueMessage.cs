using System.Text.Json.Serialization;

namespace EventGridApp.Model.Queue
{
    public class InvoiceItemQueueMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
