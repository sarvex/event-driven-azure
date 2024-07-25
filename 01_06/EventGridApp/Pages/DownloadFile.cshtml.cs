using Invoices.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventGridApp.Pages
{
    public class DownloadFileModel : PageModel
    {
       
        private readonly ILogger<IndexModel> _logger;
        private readonly IInvoiceFileService _invoiceFileService;

        public DownloadFileModel(ILogger<IndexModel> logger, IInvoiceFileService invoiceFileService)
        {
            _logger = logger;
            _invoiceFileService = invoiceFileService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var url=await _invoiceFileService.CreateDownloadLink(this.FileName);
            return Redirect(url);
        }

        [FromQuery(Name = "fileName")]
        public string FileName { get; set; }
      


       
    }
}