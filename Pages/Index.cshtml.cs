using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class IndexModel : PageModel {
        
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration) {
            this._logger = logger;
        }

        public void OnGet() {
            this._logger.LogWarning($"OnGet /Index");
        }
    }
}