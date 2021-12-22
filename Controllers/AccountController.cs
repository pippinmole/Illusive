using System.Threading.Tasks;
using AutoMapper;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Illusive.Controllers {
    [ApiController]
    [Route("api/v1/users")]
    public class AccountController : Controller {
        private readonly ILogger<AccountController> _logger;
        private readonly IAppUserManager _userManager;
        private readonly IMapper _mapper;

        public AccountController(ILogger<AccountController> logger, IAppUserManager userManager, IMapper mapper) {
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>Gets account information given a user id</summary>
        /// <remarks>
        /// <para>This request can be ran anonymously, but rate limiting obviously applies here.\</para>
        /// <para><b>Sample Curl usage:</b>
        /// 
        ///     curl https://localhost:80/api/v1/users/{id}
        ///     
        /// </para>
        /// </remarks>
        /// <param name="id">The id of the requested user
        /// </param>
        /// <response code="200">OK Request</response>
        /// <response code="204">No user found with provided id</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(string id) {
            _logger.LogInformation($"Getting account information for id {id}");

            var user = await _userManager.GetSafeUserByIdAsync(id);
            if ( user == null )
                return NoContent();

            return new JsonResult(user);
        }
        
        /// <summary>
        /// Follows the user given a unique user id
        /// </summary>
        /// <returns></returns>
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FollowUserAsync(string id) {
            var user = await _userManager.GetUserByIdAsync(User.GetUniqueId());
            if ( user == null )
                return BadRequest("Bad Token");
            
            var targetUser = await _userManager.GetUserByIdAsync(id);
            if ( targetUser == null )
                return BadRequest("Invalid target user");

            if ( targetUser.Followers.Contains(user.Id.ToString()) ) {
                _logger.LogDebug($"{user.UserName} is now following {targetUser.UserName}");
                targetUser.Followers.Remove(user.Id.ToString());
                user.Following.Add(targetUser.Id.ToString());
            } else {
                _logger.LogDebug($"{user.UserName} is now unfollowing {targetUser.UserName}");
                targetUser.Followers.Add(user.Id.ToString());
                user.Following.Remove(targetUser.Id.ToString());
            }

            await _userManager.UpdateUserAsync(targetUser);
            await _userManager.UpdateUserAsync(user);

            return Redirect($"/Account/{id}");
        }
    }
}