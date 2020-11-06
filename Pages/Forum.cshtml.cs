using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Illusive.Illusive.Utilities.Forum_Filters;
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

        public async Task<List<ForumData>> GetForums() {
            var posts = await this._forumService.GetForumDataAsync();
            var orderByParam = this.HttpContext.Request.Query["OrderBy"];
            return posts.FilterBy(orderByParam);
        }
    }
}