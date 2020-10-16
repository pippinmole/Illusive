using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class IndexModel : PageModel {
        public IConfiguration Configuration { get; }
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration) {
            this.Configuration = configuration;
            this._logger = logger;
        }

        public void OnGet() {
            this._logger.LogWarning($"Index: ConnString: {this.Configuration.GetConnectionString("AppConfig")}");
            Console.WriteLine($"OnGet Index IsAuth: {this.User.Identity.IsAuthenticated}");
        }
    }
}