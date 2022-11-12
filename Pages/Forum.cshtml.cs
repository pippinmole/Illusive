using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Models;
using Illusive.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class ForumModel : PageModel {

        private readonly ILogger<ForumModel> _logger;
        private readonly IForumService _forumService;

        public ForumResult Forums { get; set; }

        [BindProperty]
        public ForumFilter Filters { get; set; } = new();
        
        public ForumModel(ILogger<ForumModel> logger, IConfiguration configuration, IForumService forumService) {
            _logger = logger;
            _forumService = forumService;
        }

        public async Task<IActionResult> OnGetAsync() {
            var orderByParam = HttpContext.Request.Query["orderBy"];
            var pageCount = HttpContext.Request.Query["pageCount"];
            var filters = HttpContext.Request.Query["tags"];
            
            if ( string.IsNullOrEmpty(orderByParam) ) {
                return RedirectToPage("/Forum", new {orderBy = "desc"})
                    .PreserveQueryParameters(HttpContext);
            }

            if ( string.IsNullOrEmpty(pageCount) || !int.TryParse(pageCount, out var pageCountQuery) || pageCountQuery < 0 || pageCountQuery > 30 ) {
                return RedirectToPage("/Forum", new {
                    pageCount = 1
                }).PreserveQueryParameters(HttpContext);
            }

            Forums = await GetForumsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateFiltersAsync() {
            _logger.LogInformation("OnPostUpdateFiltersAsync");
            _logger.LogInformation($"Input field: {Filters.Tags}");
            
            return await OnGetAsync();
        }

        public class ForumFilter {
            public string Tags { get; set; }
            public string OrderBy { get; set; }
            public int PageNumber { get; set; }
        }
        
        public class ForumResult {
            public readonly int Page;
            public readonly int PageCount;
            public readonly IEnumerable<ForumData> Posts;

            public ForumResult(int page, int pageCount, IEnumerable<ForumData> posts) {
                Page = page;
                PageCount = pageCount;
                Posts = posts;
            }
        }
        
        /// <summary>
        /// <para> Returns all forms using the applied query strings.</para>
        /// <para> NOTE: Query strings are sanitised OnGet(), so treat all queries as valid. </para>
        /// </summary>
        /// <returns></returns>
        private async Task<ForumResult> GetForumsAsync() {
            // Accept Query parameters
            var tags = GetTagsFromQuery(Filters.Tags);
            //var tags = GetTagsFromQuery(HttpContext.Request.Query);

            const int forumLength = 15;

            // Filter post
            var allPosts = await _forumService.GetForumDataAsync();
            var filteredPosts = allPosts
                .OrderBy(Filters.OrderBy)
                .Test(Filters.PageNumber, forumLength)
                .WithTags(tags);
            
            var pageCount = (int)Math.Ceiling(allPosts.Count / (float)forumLength);
            
            // Return a result to the page
            return new ForumResult(Filters.PageNumber, pageCount, filteredPosts);
        }

        private static string[] GetTagsFromQuery(string str) {
            //var str = collection["tags"];
            //return str.Count == 0 ? null : str[0]?.Split(",");
            return string.IsNullOrEmpty(str) ? null : str.Split(",");
        }
    }
}