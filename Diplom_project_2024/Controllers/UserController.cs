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
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HousesDBContext context;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly BlobServiceClient client;
        private readonly BlobContainerClient container;

        public UserController(HousesDBContext context, UserManager<User> userManager, IMapper mapper, BlobServiceClient client)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
            this.client = client;
            container = this.client.GetBlobContainerClient("userimages");
            container.CreateIfNotExists();
            container.SetAccessPolicy(PublicAccessType.Blob);
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
            userDTO.FavoriteHouses = context.FavoriteHouses.Include(t=>t.House).Where(t=>t.UserId == user.Id).Select(t=>mapper.Map<HouseDTO>(t.House)).ToList();
            return Ok(userDTO);
        }
        [Authorize]
        [HttpPatch("SetPaymentData")]
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
        [HttpPatch("SetFirstName")]
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
        [HttpPatch("SetSurname")]
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
        [HttpPatch("SetGender")]
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
        [HttpPatch("SetDateOfBirth")]
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
        [HttpPatch("SetPhoneNumber")]
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
        [HttpPatch("SetContactEmail")]
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

        [Authorize]
        [HttpGet("GetProfileInfo")]
        public async Task<IActionResult> GetProfileInfo()
        {
            var user = await UserFunctions.GetUser(userManager, User);
            UserProfileInfoDTO userProfileDTO = mapper.Map<UserProfileInfoDTO>(user);
            var payment = await context.PaymentDatas.FirstOrDefaultAsync(t=>t.UserId==user.Id);
            if(payment != null)
            {
                userProfileDTO.cardNumber = payment.CardNumber;
                userProfileDTO.expireDate = payment.ExpireDate;
                userProfileDTO.cvv = payment.CVV;
            }
            var houses = context.Houses.Include(t=>t.Comments).Where(t=>t.UserId==user.Id).ToList();
            userProfileDTO.countOfHouses = houses.Count;
            int countOfComments=0;
            houses.ForEach(t => countOfComments+= t.Comments.Count);
            userProfileDTO.countOfComments = countOfComments;
            userProfileDTO.FavoriteHouses = context.FavoriteHouses.Include(t => t.House).Where(t => t.UserId == user.Id).Select(t => mapper.Map<HouseDTO>(t.House)).ToList();
            return Ok(userProfileDTO);
        }
        [Authorize]
        [HttpPatch("SetProfileInfo")]
        public async Task<IActionResult> SetProfileInfo(UserProfileInfoSetDTO profileInfo)
        {
            var user = await UserFunctions.GetUser(userManager, User);
            if(profileInfo.contactEmail != null) user.ContactEmail = profileInfo.contactEmail;
            if(profileInfo.surname != null) user.Surname = profileInfo.surname;
            if(profileInfo.firstName!=null) user.FirstName = profileInfo.firstName;
            if(profileInfo.gender!=null) user.Gender = profileInfo.gender;
            if(profileInfo.phoneNumber!=null) user.PhoneNumber = profileInfo.phoneNumber;
            if(profileInfo.dateOfBirth!=null) user.DateOfBirth = profileInfo.dateOfBirth;

            //if (profileInfo.image != null)
            //{
            //    if (user.ImagePath != null)
            //    {
            //        BlobContainerFunctions.DeleteImage(container, user.ImagePath);
            //    }
            //    var url = await BlobContainerFunctions.UploadImage(container, profileInfo.image);
            //    user.ImagePath = url;
            //}
            await userManager.UpdateAsync(user);
            return Ok();
        }
        [Authorize]
        [HttpPatch("SetProfileImage")]
        public async Task<IActionResult> SetProfileImage(IFormFile? image)
        {
            var user = await UserFunctions.GetUser(userManager, User);
            if (image != null)
            {
                if (user.ImagePath != null)
                {
                    BlobContainerFunctions.DeleteImage(container, user.ImagePath);
                }
                var url = await BlobContainerFunctions.UploadImage(container, image);
                user.ImagePath = url;
                await userManager.UpdateAsync(user);
            }
            return Ok();
            
        }
        [Authorize]
        [HttpDelete("DeletePaymentData")]
        public async Task<IActionResult> DeletePaymentData()
        {
            var user = await UserFunctions.GetUser(userManager, User);
            var payment = await context.PaymentDatas.FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (payment == null) return Ok();
            context.PaymentDatas.Remove(payment);
            await context.SaveChangesAsync();
            return Ok();
        }
        [Authorize]
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword(PasswordChangeDTO passwordChangeDTO)
        {
            var user = await UserFunctions.GetUser(userManager, User);
            var res = await userManager.ChangePasswordAsync(user,passwordChangeDTO.oldPassword,passwordChangeDTO.password);
            if (res.Succeeded)
            {
                return Ok();
            }
            return BadRequest(new Error("Wrong password"));
        }
        [Authorize]
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await UserFunctions.GetUser(userManager, User);
            var res = await userManager.DeleteAsync(user);
            if (res.Succeeded)
                return Ok();
            return BadRequest(new Error("Unknown error"));
        }

    }
}
