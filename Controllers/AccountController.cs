using System.Threading.Tasks;
using AutoMapper;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Pages;
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
            this._logger = logger;
            this._userManager = userManager;
            this._mapper = mapper;
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
            this._logger.LogInformation($"Getting account information for id {id}");

            var user = await this._userManager.GetSafeUserByIdAsync(id);
            if ( user == null )
                return this.NoContent();

            return new JsonResult(user);
        }
        
        /// <summary>
        /// Follows the user given a unique user id
        /// </summary>
        /// <returns></returns>
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FollowUserAsync(string id) {
            this._logger.LogError(this.User.GetUniqueId().ToString());
            
            var user = await this._userManager.GetUserByIdAsync(this.User.GetUniqueId());
            if ( user == null )
                return this.BadRequest("Bad Token");
            
            var targetUser = await this._userManager.GetUserByIdAsync(id);
            if ( targetUser == null )
                return this.BadRequest("Invalid target user");

            if ( targetUser.Followers.Contains(user.Id.ToString()) ) {
                this._logger.LogDebug($"{user.UserName} is now following {targetUser.UserName}");
                targetUser.Followers.Remove(user.Id.ToString());
                user.Following.Add(targetUser.Id.ToString());
            } else {
                this._logger.LogDebug($"{user.UserName} is now unfollowing {targetUser.UserName}");
                targetUser.Followers.Add(user.Id.ToString());
                user.Following.Remove(targetUser.Id.ToString());
            }

            await this._userManager.UpdateUserAsync(targetUser);
            await this._userManager.UpdateUserAsync(user);

            this._logger.LogDebug($"{targetUser.UserName} has followers: {string.Concat(targetUser.Followers)}");
            return this.Redirect($"/Account/{id}");
        }
    }
}