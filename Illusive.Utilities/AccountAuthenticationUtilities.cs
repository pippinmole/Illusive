using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Illusive.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Illusive.Utility {
    public static class AccountAuthenticationUtilities {

        public static Guid GetUniqueId(this ClaimsPrincipal principal) {
            var name = principal.FindFirst(ClaimTypes.NameIdentifier);
            return name == null ? Guid.Empty : new Guid(name.Value);
        }
        
        public static string GetDisplayName(this ClaimsPrincipal principal) {
            return principal.FindFirst(ClaimTypes.Name)?.Value;
        }
        
        public static bool IsAdminAccount(this ClaimsPrincipal principal) {
            var roles = principal.FindAll(ClaimTypes.Role);
            return roles.Any(x => x.Value == "Admin");
        }
        
        public static bool IsStandardAccount(this ClaimsPrincipal principal) {
            var roles = principal.FindAll(ClaimTypes.Role);
            return roles.Any(x => x.Value == "Standard");
        }
        
        public static bool IsLoggedIn(this ClaimsPrincipal principal) {
            return principal.Identity.IsAuthenticated;
        }
        
        public static bool CanDeletePost(this ClaimsPrincipal principal, ForumData post) {
            return post.OwnerId == principal.GetUniqueId();
        }
        
        public static bool CanEditPost(this ClaimsPrincipal principal, ForumData post) {
            return post.OwnerId == principal.GetUniqueId();
        }
        
        public static bool CanLockPost(this ClaimsPrincipal principal, ForumData post) {
            return post.OwnerId == principal.GetUniqueId();
        }
        
        public static bool CanDeleteReply(this ClaimsPrincipal principal, ForumReply post) {
            return post.AuthorId == principal.GetUniqueId();
        }

        public static void GenerateApiKey(this ApplicationUser user, IConfiguration configuration) {
            var securityKey = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]);

            var claims = new[] {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var credentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Today,
                signingCredentials: credentials);
            
            user.ApiKey = new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}