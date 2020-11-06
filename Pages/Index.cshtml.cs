using System;
using System.Collections.Generic;
using Illusive.Illusive.Authentication.Utility;
using Illusive.Illusive.Cdn.Interfaces;
using Illusive.Illusive.Database.Extension_Methods;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class IndexModel : PageModel {

        private readonly ILogger<IndexModel> _logger;
        private readonly IAccountService _accountService;
        private readonly IContentService _contentService;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, IAccountService accountService,
            IContentService contentService) {
            this._logger = logger;
            this._accountService = accountService;
            this._contentService = contentService;
        }

        public AccountData GetAccount() {
            var accountId = this.User.GetUniqueId();
            return this._accountService.GetAccountById(accountId);
        }
    }
}