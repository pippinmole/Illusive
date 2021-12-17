using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class IndexModel : PageModel {

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger) {
            this._logger = logger;
        }

        public IActionResult OnGet() {
            return this.LocalRedirect("/Forum");
        }
    }
}