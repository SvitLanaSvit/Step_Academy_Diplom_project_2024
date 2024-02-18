using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
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
        public AuthorizationController(HousesDBContext context, UserManager<User> manager, IConfiguration configuration, SignInManager<User> signInManager)
        {
            this.manager = manager;
            this.context = context;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }
        [HttpPost("Registration")]
        public async Task<IActionResult> Registration(UserRegisterDTO user)
        {  
            if(ModelState.IsValid)
            {
                var createdUser = new User() { UserName = user.UserName, Email = user.Email, DisplayName = user.DisplayName };
                var check = await manager.FindByEmailAsync(createdUser.Email);
                if(check != null)
                {
                    return BadRequest("This Email already taken");
                }
                var res = await manager.CreateAsync(createdUser, user.Password);
                
                if (res.Succeeded)
                {
                    // Генерируем секретный ключ на основе конфигурации
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

                    // Создаем JWT токен
                    var token = new JwtSecurityToken(
                        issuer: configuration["Jwt:Issuer"],
                        audience: configuration["Jwt:Audience"],
                        expires: DateTime.UtcNow.AddHours(1), // Срок действия токена (1 час)
                        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                    return Ok(
                        new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token)
                        }
                        );
                }
                return BadRequest(res.Errors);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO user)
        {
            if(ModelState.IsValid)
            {
                var res = await signInManager.PasswordSignInAsync(user.Username, user.Password, user.RememberMe, false);
                if (res.Succeeded)
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                    var us = await manager.FindByNameAsync(user.Username);
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, us.Id));
                    claims.Add(new Claim(ClaimTypes.Name, us.UserName));

                    if (User.IsInRole("Admin"))
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                    var token = new JwtSecurityToken(
                        issuer: configuration["Jwt:Issuer"],
                        audience: configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(1), // Срок действия токена (1 час)
                        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                    // Возвращаем JWT токен в ответе
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }
                return BadRequest("Wrong login or password!");
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
