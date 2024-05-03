using Azure.Core;
using Diplom_project_2024.CustomErrors;
using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
using Diplom_project_2024.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly Services.IAuthenticationService authentication;

        public AuthorizationController(HousesDBContext context, UserManager<User> manager, IConfiguration configuration, SignInManager<User> signInManager, Services.IAuthenticationService authentication)
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
                        var token = await authentication.CreateToken();
                        return Ok(token);
                    }    
                }
                catch(ErrorException ex)
                {
                    return BadRequest(ex.GetErrors()); 
                }
            }
            return BadRequest(new Error("Required fields were not specified"));
        }
        [HttpGet("LoginGoogle")]
        public IActionResult Login(string returnUrl = "/")
        {
            //var properties = new AuthenticationProperties { RedirectUri = returnUrl };
            //return Challenge(properties, "Google");

            string redirecturl = Url.ActionLink("GoogleResponse", "Authorization");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirecturl);
            var chall = new ChallengeResult("Google", properties);
            return chall    ;
        }   
        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");
            if (!authenticateResult.Succeeded)
            {
                return BadRequest(new Error( "Failed to authenticate with Google" ));
            }
            var user = authenticateResult.Principal;
            authentication.LoginOrCreateUser(user);
            var token = authentication.CreateToken();
            return Ok(token);
        }
        //[HttpGet("LoginFacebook")]
        //public IActionResult LoginFacebook(string returnUrl = "/")
        //{
        //    string redirecturl = Url.ActionLink("LoginFacebook", "Authorization");
        //    var properties = new AuthenticationProperties { RedirectUri = redirecturl };
        //    return Challenge(properties, "Facebook");
        //}
        [HttpGet("LoginFacebook")]
        public IActionResult LoginFacebook(string returnUrl = "/")
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Facebook", Url.Action("LoginFacebook", "Authorization", new { returnUrl }));
            return Challenge(properties, "Facebook");
        }

        [HttpGet("FacebookCallback")]
        public async Task<IActionResult> FacebookCallback(string returnUrl = "/")
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("Error loading external login information.");
            }

            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return BadRequest("User account locked out.");
            }
            else
            {
                //var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                //var user = new User { UserName = email, Email = email };
                //var identityResult = await userManager.CreateAsync(user);
                //if (identityResult.Succeeded)
                //{
                //    identityResult = await userManager.AddLoginAsync(user, info);
                //    if (identityResult.Succeeded)
                //    {
                //        await _signInManager.SignInAsync(user, isPersistent: false);
                //        return Redirect(returnUrl);
                //    }
                //}
                return BadRequest("Failed to create user.");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO user)
        {
            if (ModelState.IsValid)
            {
                if (!await authentication.ValidateUser(user)) return Unauthorized(new Error("Wrong email or password"));
                var tokenDto = await authentication.CreateToken();
                return Ok(tokenDto);
            }
            return BadRequest(new Error("Required fields were not specified"));
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetAdmin")]
        public async Task< IActionResult> GetAdmin()
        {
            var user = await manager.GetUserAsync(User);
            await manager.AddToRoleAsync(user, "Admin");
            return Ok();
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(RefreshTokenDTO refreshToken)
        {
            var refToken = await context.RefreshTokens.FirstOrDefaultAsync(t=> t.refreshToken== refreshToken.refreshToken);
            if (refToken == null) return BadRequest(new Error("Invalid token"));
            context.RefreshTokens.Remove(refToken);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
