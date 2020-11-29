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

        [HttpGet("{id}")]
        public IActionResult GetForumPost(string id) {
            var result = this._forumService.GetForumWhere(x => x.Id == id);

            return this.View("Account");            
            
            return new JsonResult(result);
        }
    }
}