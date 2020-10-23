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

            var pHash = BCrypt.Net.BCrypt.HashPassword("passw@rd");
            var isValid1 = BCrypt.Net.BCrypt.Verify("passw@rd", pHash);
            var isValid2 = BCrypt.Net.BCrypt.Verify("passw%25rd", pHash);
            
            this._logger.LogWarning($"Hash is valid 1: {isValid1}");
            this._logger.LogWarning($"Hash is valid 1: {isValid2}");
            
            this._logger.LogWarning($"OnGet /Index");
        }
    }
}