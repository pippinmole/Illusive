﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Models;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class ForumModel : PageModel {

        private readonly ILogger<ForumModel> _logger;
        private readonly IForumService _forumService;

        public ForumModel(ILogger<ForumModel> logger, IConfiguration configuration, IForumService forumService) {
            this._logger = logger;
            this._forumService = forumService;
        }

        public IActionResult OnGet() {
            var orderByParam = this.HttpContext.Request.Query["OrderBy"];
            if ( string.IsNullOrEmpty(orderByParam) ) {
                return this.RedirectToPage("/Forum", new { orderby = "views" });
            }

            return this.Page();
        }

        public async Task<List<ForumData>> GetForums() {
            var posts = await this._forumService.GetForumDataAsync();
            var orderByParam = this.HttpContext.Request.Query["OrderBy"];
            return posts.FilterBy(orderByParam);
        }
    }
}