using Diplom_project_2024.Data;
using Diplom_project_2024.Functions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HousesDBContext context;
        private readonly UserManager<User> userManager;

        public UserController(HousesDBContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserId()
        {
            var user = await UserFunctions.GetUser(userManager, User);
            if (user == null) return NotFound();
            return Ok(user.Id);
        }
    }
}
