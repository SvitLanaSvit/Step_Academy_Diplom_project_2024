using Diplom_project_2024.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Diplom_project_2024.Functions
{
    public static class UserFunctions
    {
        public static async Task< User> GetUser(UserManager<User> userManager, ClaimsPrincipal User)
        {
            return await userManager.FindByNameAsync(User.Identity.Name);
        }
    }
}
