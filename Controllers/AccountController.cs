using System.Threading.Tasks;
using AutoMapper;
using Illusive.Illusive.Core.User_Management.Interfaces;
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
    }
}