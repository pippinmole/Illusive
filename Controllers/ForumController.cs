﻿using System;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Models;
using Illusive.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Illusive.Controllers {
    [ApiController]
    [Route("api/v1/forum")]
    public class ForumController : Controller {
        private readonly ILogger<ForumController> _logger;
        private readonly IForumService _forumService;
        private readonly IAppUserManager _userManager;

        public ForumController(ILogger<ForumController> logger, IForumService forumService, IAppUserManager userManager) {
            this._logger = logger;
            this._forumService = forumService;
            this._userManager = userManager;
        }

        /// <summary>Returns the forum post data from a forum id</summary>
        /// <remarks>
        /// <para>This request can be ran anonymously, but rate limiting obviously applies here.\</para>
        /// <para><b>Sample Curl usage:</b>
        /// 
        ///     curl https://localhost:80/api/v1/forum/{id}
        ///     
        /// </para>
        /// </remarks>
        /// <param name="id">The id of the requested forum post.
        /// </param>
        /// <returns>A forum post and all attributed data.</returns>
        /// <response code="200">Returns a valid forum post</response>
        /// <response code="400">Returns a null response</response>
        [HttpGet("{id}")]
        public ForumData GetForumPost(string id) {
            var result = this._forumService.GetForumWhere(x => x.Id == id);
            return result;
        }

        /// <summary>Returns a random forum post</summary>
        /// <remarks>
        /// <para>This request can be ran anonymously, but rate limiting obviously applies here.\</para>
        /// <para><b>Sample Curl usage:</b>
        /// 
        ///     curl https://localhost:80/api/v1/forum/getrandom
        ///     
        /// </para>
        /// </remarks>
        /// <returns>A forum post and all attributed data</returns>
        /// <response code="200">Returns a valid forum post</response>
        /// <response code="400">Returns a null response</response>
        [HttpGet("getrandom")]
        public ForumData GetRandomForum() {
            var random = new Random().Next(0, this._forumService.CollectionSize());
            var result = this._forumService.GetForumIndex(random);
            return result;
        }
        
        /// <summary>
        /// Creates a forum post
        /// </summary>
        /// <remarks>
        /// <para>You will need to attach your profile API key to the header, in the format 'Authorization': '[api key]'. The API key can be found in the User Account settings.\</para>
        /// <para><b>Please note: </b>The API will only accept user-controller properties, such as Title, Content and Tags, exactly like what you see through the website;
        /// properties such as Views, Comments etc will be ignored.\</para></remarks>
        /// <param name="forumData"></param>
        /// <response code="200">Successfully created the forum post</response>
        /// <response code="400">Authorization has been denied for this request</response>
        /// <response code="400">Bad request</response>
        /// <returns></returns>
        [HttpPost("createpost")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreatePost([FromBody] ForumData forumData) {
            this._logger.LogWarning("post");
            var user = await this._userManager.GetUserByIdAsync(this.User.GetUniqueId());
            if ( user == null )
                return this.BadRequest("Bad Token");

            // Sanitise forum data from alteration of non-user-controlled properties. 
            var forumPost = new ForumData {
                Title = forumData.Title,
                Content = forumData.Content,
                Tags = forumData.Tags,
                OwnerId = user.Id
            };

            await this._forumService.AddForumPostAsync(forumPost);
            
            this._logger.LogInformation($"{user.UserName} has created a new forum post through the WebAPI: {forumPost.Title}");

            return new JsonResult(forumPost);
        }
    }
}
