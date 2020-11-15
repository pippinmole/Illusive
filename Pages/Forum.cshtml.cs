using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var pageCount = this.HttpContext.Request.Query["pageCount"];
            if ( string.IsNullOrEmpty(pageCount) || !int.TryParse(pageCount, out var pageCountQuery) || pageCountQuery < 0 || pageCountQuery > 30) {
                return this.RedirectToPage("/Forum", new {orderby = orderByParam, pageCount = 1});
            }

            return this.Page();
        }

        public class ForumResult {
            
            public readonly int Page;
            public readonly int PageCount;
            public readonly IEnumerable<ForumData> Posts;

            public ForumResult(int page, int pageCount, IEnumerable<ForumData> posts) {
                this.Page = page;
                this.PageCount = pageCount;
                this.Posts = posts;
            }
        }
        
        /// <summary>
        /// <para> Returns all forms using the applied query strings.</para>
        /// <para> NOTE: Query strings are sanities OnGet(), so treat all queries as valid. </para>
        /// </summary>
        /// <returns></returns>
        public async Task<ForumResult> GetForumsAsync() {
            // Accept Query parameters
            var order = this.HttpContext.Request.Query["OrderBy"];
            var pageNumberParam = Convert.ToInt32(this.HttpContext.Request.Query["pageCount"]);
            var forumLength = 15;

            // Filter post
            var allPosts = await this._forumService.GetForumDataAsync();
            var filteredPosts = allPosts.OrderBy(order).Skip((pageNumberParam - 1) * forumLength).Take(forumLength);
            var pageCount = (int)Math.Ceiling(allPosts.Count() / (float)forumLength);
            
            // Return a result to the page
            return new ForumResult(pageNumberParam, pageCount, filteredPosts);
        }
    }
}