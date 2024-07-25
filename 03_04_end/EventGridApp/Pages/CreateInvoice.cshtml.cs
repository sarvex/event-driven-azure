using Invoices.Data.Models.Invoices;
using Invoices.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventGridApp.Pages
{
    public class CreateInvoiceModel : PageModel
    {
        private readonly static Random _randomAmount= new Random();
        private readonly static Random _randomInvoiceCode= new Random();

        private readonly ILogger<IndexModel> _logger;
        private readonly IDataService _dataService;
        private readonly IInvoiceFileService _invoiceFileService;

        public CreateInvoiceModel(ILogger<IndexModel> logger, IDataService dataService, IInvoiceFileService invoiceFileService)
        {
            _logger = logger;
            _dataService = dataService;
            _invoiceFileService= invoiceFileService;
        }

       

        [BindProperty]
        public string ClientName { get; set; }
        [BindProperty]
        public string ClientEmail { get; set; }
        [BindProperty]
        public decimal Amount { get; set; }
        [BindProperty]
        public string InvoiceCode { get; set; }
        [BindProperty]
        public DateTime Date { get; set; }
        [BindProperty]
        public string InvoiceDescription { get; set; }
        public void OnGet()
        {
            this.LoadDummyData();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var newInvoice=  await _dataService.AddInvoice(new InvoiceData() {  InvoiceCode = InvoiceCode, Amount = Amount, ClientEmail=ClientEmail, ClientName=ClientName, InvoiceDescription= InvoiceDescription, Id=Guid.NewGuid().ToString() });
            
            return RedirectToPage(pageName: "InvoiceDetails", new  { id=newInvoice.Id});
        }
        private void LoadDummyData()
        {
            this.ClientName = "John Smith";
            this.ClientEmail = "jsmith@gmail.com";
            this.Amount = _randomAmount.Next(100, 1000);
            this.InvoiceCode = "I-" + _randomInvoiceCode.Next();
            this.Date = DateTime.Today;
            this.InvoiceDescription = $"For services rendered for the month of {this.Date.ToString("MMMM")}";
        }
    }
}