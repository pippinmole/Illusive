using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Illusive.Illusive.Core.User_Management.Extension_Methods {
    public static class UserManagerRoles {

        public static async Task<bool> IsUserAdmin(this IAppUserManager userManager, ApplicationUser user) {
            return await userManager.IsUserInRole(user, "Admin");
        }
        
        public static async Task SetInitialRolesAsync(this RoleManager<ApplicationRole> roleManager, IEnumerable<string> roles) {
            if ( roleManager == null ) throw new NullReferenceException(nameof(roleManager));

            foreach ( var role in roles ) {
                if(await roleManager.RoleExistsAsync(role))
                    continue;
                
                await roleManager.CreateAsync(new ApplicationRole(role));
            }
        }
    }
}