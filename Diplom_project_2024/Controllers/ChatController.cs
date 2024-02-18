using AutoMapper;
using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly HousesDBContext context;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public ChatController(HousesDBContext context,UserManager<User> userManager,IMapper mapper) 
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var chats = context.Chats.ToList();
            return Ok(chats);
        }
        [Authorize]
        [HttpGet("GetCurrentUserChats")]
        public async Task<IActionResult> GetCurrentUserChats()
        {
            var user = await userManager.FindByNameAsync(this.User.Identity.Name.ToString());
            if (user == null) return NotFound();
            var chats = context.Chats.Include(t=>t.Users).Where(t=>t.Users.Contains(user)).Select(t=>new ChatDTO() 
            { 
                ChatWith = mapper.Map<UserDTO>(t.Users.First(t => t.Id != user.Id)),
                Id = t.Id,
                LastMessage = mapper.Map<MessageDTO>(t.Messages.OrderByDescending(m=>m.SendingTime).First()),
                CountOfUnreadMessages = t.Messages.Where(m=>m.FromUser!=user&&m.IsRead==false).Count() 
            }).ToList();
            return Ok(chats);
        }

    }
}
