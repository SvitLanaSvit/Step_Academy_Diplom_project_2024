using AutoMapper;
using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly HousesDBContext context;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public MessageController(HousesDBContext context, UserManager<User> userManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }


        [Authorize]
        [HttpGet("ByChatId/{Id}")]
        public IActionResult GetMessagesByChatId(int Id) 
        {
            var messages = context.Messages.Where(t=>t.ChatId == Id).ToList().Select(m =>
            //new MessageDTO()
            //{
            //    Id = m.Id,
            //    Content = m.Content,
            //    SendingTime = m.SendingTime,
            //    FromUser = mapper.Map<UserDTO>(m.FromUser),
            //    IsRead = m.IsRead,

            //}
             mapper.Map<MessageDTO>(m)
            ).OrderBy(t=>t.SendingTime).ToList();
            return Ok(messages);

        }
        [Authorize]
        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessageAPI(MessageSendDTO sentMessage)
        {
            if(ModelState.IsValid)
            {
                if (sentMessage.ToUserId == null && sentMessage.ChatId == null) return BadRequest("ChatId and UserId at least one of these needed");
                var currentUser = await userManager.FindByNameAsync(User.Identity.Name);
                if (currentUser == null) return Unauthorized();
                if(sentMessage.ChatId!= null)
                {
                    var chat = await context.Chats.Include(t=>t.Messages).FirstOrDefaultAsync(t=>t.Id==sentMessage.ChatId);
                    if (chat == null) return NotFound($"Chat with id {sentMessage.ChatId} wasn't found!");

                    await SendMessage(sentMessage,chat,currentUser, context);
                    return Ok("Message was sent!");
                }
                else
                {
                    var toUser = await userManager.FindByIdAsync(sentMessage.ToUserId);
                    if (toUser == null) return NotFound($"User with id {sentMessage.ToUserId} wasn't found!");
                    List<User> users = new List<User>() { currentUser, toUser };
                    Chat chat = new Chat()
                    {
                        Users = users,
                        Messages = new List<Message>()
                    };
                    context.Chats.Add(chat);
                    await context.SaveChangesAsync();
                    await SendMessage(sentMessage, chat, currentUser,context);
                    return Ok("Message was sent!");
                }
            }
            return BadRequest(ModelState);
        }
        private async Task SendMessage( MessageSendDTO sentMessage, Chat chat, User currentUser, HousesDBContext dbContext)
        {
            Message message = new Message()
            {
                Chat = chat,
                ChatId = chat.Id,
                Content = sentMessage.Content,
                FromUser = currentUser,
                FromUserId = currentUser.Id,
                IsRead = false,
                SendingTime = DateTime.Now
            };
            dbContext.Messages.Add(message);
            await dbContext.SaveChangesAsync();
            chat.Messages.Add(message);
            dbContext.Chats.Update(chat);
            await dbContext.SaveChangesAsync();
        }
    }
    
}
