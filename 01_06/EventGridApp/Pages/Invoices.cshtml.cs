
using Invoices.Data.Models.Invoices;
using Invoices.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventGridApp.Pages
{
    public class InvoicesModel : PageModel
    {
       
        private readonly ILogger<IndexModel> _logger;
        private readonly IDataService _dataService;

        public InvoicesModel(ILogger<IndexModel> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public IEnumerable<InvoiceData> Invoices;
        public async Task OnGetAsync()
        {

            this.Invoices = await _dataService.GetAllInvoices();
        }
     

    }
}