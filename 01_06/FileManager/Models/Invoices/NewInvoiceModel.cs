using Newtonsoft.Json;

namespace Invoices.Data.Models.Invoices
{

    public class InvoiceData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string InvoiceCode { get; set; }
        public decimal Amount { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string InvoiceDescription { get; set; }

        public DateTime? Date { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
