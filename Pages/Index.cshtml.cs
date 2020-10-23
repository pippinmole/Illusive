using System;
using Illusive.Illusive.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class IndexModel : PageModel {
        
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration) {
            this._logger = logger;
        }

        public async void OnGetAsync() {
            
            var isEmail = "nick@ruffles.dev".IsEmail();
            this._logger.LogWarning($"IsEmail: {isEmail}");
            this._logger.LogWarning($"OnGet /Index");
        }
    }
}