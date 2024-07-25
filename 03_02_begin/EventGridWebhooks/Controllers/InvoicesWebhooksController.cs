using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.AspNetCore.Mvc;

namespace EventGridWebhooks.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class InvoicesWebhooksController : ControllerBase
    {


        [HttpPost(Name = "InvoiceWebHooks")]
        public IActionResult Post([FromBody] EventGridEvent[] eventGridEvents)
        {
            foreach (var eventGridEvent in eventGridEvents)
            {
                
                Console.WriteLine("Event: " + eventGridEvent.EventType);

            }
            return Ok();
        }
    }
}