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
using Microsoft.EntityFrameworkCore;

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
        [Authorize]
        [HttpPost("SetPaymentData")]
        public async Task<IActionResult> SetPaymentData(PaymentDataDTO paymentDTO)
        {
            if(ModelState.IsValid)
            {
                var user = await UserFunctions.GetUser(userManager, User);
                var check = await context.PaymentDatas.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (check != null)
                {
                    check.ExpireDate= paymentDTO.ExpireDate;
                    check.CardNumber = paymentDTO.CardNumber;
                    check.CVV  = paymentDTO.CVV;
                    context.PaymentDatas.Update(check);
                    await context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    //var payment = mapper.Map<PaymentData>(paymentDTO);
                    var payment = new PaymentData() { CardNumber = paymentDTO.CardNumber, CVV = paymentDTO.CVV, ExpireDate = paymentDTO.ExpireDate };
                    payment.User = user;
                    payment.UserId = user.Id;
                    await context.PaymentDatas.AddAsync(payment);
                    await context.SaveChangesAsync();
                    var dto = mapper.Map<PaymentDataDTO>(payment);
                    return Ok();
                }
                
            }
            return BadRequest(new Error("Required fields were not specified"));
        }
        [Authorize]
        [HttpGet("GetPaymentData")]
        public async Task<IActionResult> GetPaymentData()
        {
            var user = await UserFunctions.GetUser(userManager, User);
            var payment = await context.PaymentDatas.FirstOrDefaultAsync(t=>t.UserId==user.Id);
            if (payment == null) return NotFound(new Error("User doesn't have payment data"));
            var paymentDTO = mapper.Map<PaymentDataDTO>(payment);
            return Ok(paymentDTO);
        }
    }
}
