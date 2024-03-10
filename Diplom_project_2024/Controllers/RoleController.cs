using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
          
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cant be empty");
            }
            
            var existingRole = await roleManager.FindByNameAsync(roleName);
            if (existingRole != null)
            {
                return Conflict("This role name already exists");
            }

            var newRole = new IdentityRole(roleName);
            var result = await roleManager.CreateAsync(newRole);

            if (result.Succeeded)
            {
                return Ok("Role was created");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}
