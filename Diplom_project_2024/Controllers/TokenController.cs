using Diplom_project_2024.CustomErrors;
using Diplom_project_2024.Models.DTOs;
using Diplom_project_2024.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAuthenticationService authentication;

        public TokenController(IAuthenticationService authentication)
        {
            this.authentication = authentication;
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDTO refreshTokenDTO)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var tokenDtoToReturn = await authentication.RefreshAccessToken(refreshTokenDTO.refreshToken);
                    return Ok(tokenDtoToReturn);
                }
                catch (ErrorException ex)
                {
                    return BadRequest(ex.GetErrors());
                }
            }
            return BadRequest(ModelState);
        }

    }
}
