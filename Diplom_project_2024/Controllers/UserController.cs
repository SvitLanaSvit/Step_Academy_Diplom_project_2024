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
using Diplom_project_2024.Models;

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
        [Authorize]
        [HttpPost("SetFirstName")]
        public async Task<IActionResult> SetFirstname(SetFirstNameModel firstname)
        {
            if (ModelState.IsValid)
            {
                var user = await UserFunctions.GetUser(userManager, User);
                if (!string.IsNullOrEmpty(firstname.firstName))
                {
                    user.FirstName = firstname.firstName;
                    await userManager.UpdateAsync(user);
                    return Ok();
                }
                return BadRequest(new Error("Wrong firstname"));
            }
            return BadRequest(new Error("Required fields were not specified"));
        }
        [Authorize]
        [HttpPost("SetSurname")]
        public async Task<IActionResult> SetSurname(SetSurnameModel surname)
        {
            if (ModelState.IsValid)
            {
                var user = await UserFunctions.GetUser(userManager, User);
                if (!string.IsNullOrEmpty(surname.surname))
                {
                    user.Surname = surname.surname;
                    await userManager.UpdateAsync(user);
                    return Ok();
                }
                return BadRequest(new Error("Wrong surname"));
            }
            return BadRequest(new Error("Required fields were not specified"));
        }
        [Authorize]
        [HttpPost("SetGender")]
        public async Task<IActionResult> SetGender(SetGenderModel gender)
        {
            if (ModelState.IsValid)
            {
                var user = await UserFunctions.GetUser(userManager, User);
                if (!string.IsNullOrEmpty(gender.gender))
                {
                    user.Gender = gender.gender;
                    await userManager.UpdateAsync(user);
                    return Ok();
                }
                return BadRequest(new Error("Wrong gender"));
            }
            return BadRequest(new Error("Required fields were not specified"));
        }
        [Authorize]
        [HttpPost("SetDateOfBirth")]
        public async Task<IActionResult> SetDateOfBirth(SetDateOfBirthModel dateOfBirthModel)
        {
            if (ModelState.IsValid)
            {
                if (dateOfBirthModel.dateOfBirth == null) return BadRequest(new Error("Wrong date of birth"));
                var user = await UserFunctions.GetUser(userManager, User);
                user.DateOfBirth = dateOfBirthModel.dateOfBirth;
                await userManager.UpdateAsync(user);
                return Ok();
            }
            return BadRequest(new Error("Required fields were not specified"));
        }
        [Authorize]
        [HttpPost("SetPhoneNumber")]
        public async Task<IActionResult> SetPhoneNumber(SetPhoneNumber phoneNumber)
        {
            if (ModelState.IsValid)
            {
                var user = await UserFunctions.GetUser(userManager, User);
                if (!string.IsNullOrEmpty(phoneNumber.phoneNumber))
                {
                    user.PhoneNumber = phoneNumber.phoneNumber;
                    await userManager.UpdateAsync(user);
                    return Ok();
                }
                return BadRequest(new Error("Wrong phone number"));
            }
            return BadRequest(new Error("Required fields were not specified"));
        }
        [Authorize]
        [HttpPost("SetContactEmail")]
        public async Task<IActionResult> SetContactEmail(SetContactEmail contactEmail)
        {
            if (ModelState.IsValid)
            {
                var user = await UserFunctions.GetUser(userManager, User);
                if (!string.IsNullOrEmpty(contactEmail.contactEmail))
                {
                    user.ContactEmail = contactEmail.contactEmail;
                    await userManager.UpdateAsync(user);
                    return Ok();
                }
                return BadRequest(new Error("Wrong contact email"));
            }
            return BadRequest(new Error("Required fields were not specified"));
        }
        [Authorize]
        [HttpGet("GetBasicInfo")]
        public async Task<IActionResult> GetBasicInfo()
        {
            var user = await UserFunctions.GetUser(userManager, User);
            UserBasicInfoDTO userBasicInfoDTO = new UserBasicInfoDTO()
            {
                dateOfBirth = user.DateOfBirth,
                gender = user.Gender,
                surname = user.Surname,
                firstname = user.FirstName,
            };
            return Ok(userBasicInfoDTO);
        }
        [Authorize]
        [HttpGet("GetContactInfo")]
        public async Task<IActionResult> GetContactInfo()
        {
            var user = await UserFunctions.GetUser(userManager, User);
            UserContactInfoDTO userContactInfoDTO = new UserContactInfoDTO()
            {
                phoneNumber = user.PhoneNumber,
                contactEmail = user.ContactEmail,
            };
            return Ok(userContactInfoDTO);
        }
    }
}
