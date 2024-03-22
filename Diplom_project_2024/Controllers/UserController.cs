using AutoMapper;
using Diplom_project_2024.Data;
using Diplom_project_2024.Functions;
using Diplom_project_2024.Models.DTOs;
using Diplom_project_2024.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Diplom_project_2024.CustomErrors;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HousesDBContext context;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public UserController(HousesDBContext context, UserManager<User> userManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }
        [Authorize]
        [HttpGet("GetUserId")]
        public async Task<IActionResult> GetCurrentUserId()
        {
            var user = await UserFunctions.GetUser(userManager, User);
            if (user == null) return NotFound();
            return Ok(user.Id);
        }
        [Authorize]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var user = await UserFunctions.GetUser(userManager, User);
            if(user == null) return NotFound(new ErrorException("user wasn't found").GetErrors());
            var us = await userManager.FindByIdAsync(user.Id);
            if (us == null) return NotFound(new ErrorException("user wasn't found").GetErrors());
            var userDTO = mapper.Map<UserDTO>(us);
            return Ok(userDTO);
        }
    }
}
