using Illusive.Database;
using Microsoft.AspNetCore.Mvc;

namespace Illusive.Controllers {
    [ApiController]
    [Route("api/v1/forum")]
    public class ForumController : Controller {
        private readonly IForumService _forumService;
        
        public ForumController(IForumService forumService) {
            this._forumService = forumService;
        }

        /// <summary>
        /// Returns the forum post given a valid forum id.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="id">The id of the requested forum post.</param>
        /// <returns>A forum post and all attributed data.</returns>
        /// <response code="201">Returns a valid forum post</response>
        /// <response code="400">Returns a null response</response>
        [HttpGet("{id}")]
        public IActionResult GetForumPost(string id) {
            var result = this._forumService.GetForumWhere(x => x.Id == id);
            return new JsonResult(result);
        }
    }
}