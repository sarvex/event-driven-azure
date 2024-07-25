using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventGridApp.Pages
{
    public class LiveUpdatesModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public LiveUpdatesModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        
    }
}