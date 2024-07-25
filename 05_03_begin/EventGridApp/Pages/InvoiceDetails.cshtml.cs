using Invoices.Data.Models;
using Invoices.Data.Models.Invoices;
using Invoices.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventGridApp.Pages
{
    public class InvoiceDetailsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IDataService _dataService;
        private readonly IInvoiceFileService _fileService;

        public InvoiceDetailsModel(ILogger<IndexModel> logger, IDataService dataService, IInvoiceFileService fileService)
        {
            _logger = logger;
            _dataService = dataService;
            _fileService = fileService;
        }
        public InvoiceData InvoiceDetails { get; set; }
        public IEnumerable<FileViewModel> Files { get; set; }
        public async Task OnGetAsync(string id)
        {
            this.InvoiceDetails = await _dataService.GetInvoiceDetails(id);
            this.Files= await _fileService.GetFilesForInvoice(id);
        }

       
       
    }
}