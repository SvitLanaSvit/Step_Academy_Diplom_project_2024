using Azure.Core;
using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
using Diplom_project_2024.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<User> manager;
        private readonly HousesDBContext context;
        private readonly IConfiguration configuration;
        private readonly SignInManager<User> signInManager;
        private readonly IAuthenticationService authentication;

        public AuthorizationController(HousesDBContext context, UserManager<User> manager, IConfiguration configuration, SignInManager<User> signInManager, IAuthenticationService authentication)
        {
            this.manager = manager;
            this.context = context;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.authentication = authentication;
        }
        [HttpPost("Registration")]
        public async Task<IActionResult> Registration(UserRegisterDTO user)
        {  
            if(ModelState.IsValid)
            {
                try
                {
                    var res = await authentication.RegisterUser(user);   
                    if(res)
                    {
                        var token = await authentication.CreateToken(true);
                        return Ok(token);
                    }    
                }
                catch(ErrorException ex)
                {
                    return BadRequest(ex.GetErrors());
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO user)
        {
            if (ModelState.IsValid)
            {
                if (!await authentication.ValidateUser(user)) return Unauthorized(new ErrorException("Wrong email or password"));
                var tokenDto = await authentication.CreateToken(true);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(1) // Установите необходимый срок действия куки
                };

                Response.Cookies.Append("AccessToken", tokenDto.accessToken, cookieOptions);
                return Ok(tokenDto);
            }
            return Unauthorized(ModelState);
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetAdmin")]
        public async Task< IActionResult> GetAdmin()
        {
            var user = await manager.GetUserAsync(User);
            await manager.AddToRoleAsync(user, "Admin");
            return Ok();
        }
    }
}
