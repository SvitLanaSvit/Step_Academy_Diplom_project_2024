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
        public async Task<IActionResult> Refresh(TokenDTO tokenDTO)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var tokenDtoToReturn = await authentication.RefreshToken(tokenDTO);
                    return Ok(tokenDtoToReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest(new ErrorException(ex.Message));
                }
            }
            return BadRequest(ModelState);
        }

    }
}
