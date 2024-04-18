using Diplom_project_2024.CustomErrors;
using Diplom_project_2024.Data;
using Diplom_project_2024.Functions;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentsController : ControllerBase
    {
        private readonly HousesDBContext _context;
        private readonly UserManager<User> userManager;    

        public RentsController(HousesDBContext context, UserManager<User> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        //GET: api/Rents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentDTO>>> GetRents()
        {
            List<RentDTO> rents = await _context.Rents
            .Include(r => r.House)
                .ThenInclude(h => h!.Address)
            .Include(r => r.House)
                .ThenInclude(h => h!.Category)
            .Include(r => r.User)
            .Select(r => new RentDTO
            {
                Id = r.Id,
                CountOfDay = r.CountOfDay,
                Price = r.Price,
                From = r.From,
                To = r.To,
                User = new UserDTO
                {
                    Id = r.User!.Id,
                    FirstName = r.User!.FirstName,
                    Surname = r.User!.Surname,
                    Email = r.User.Email
                },
                House = new HouseDTO
                {
                    Id = r.House!.Id,
                    Description = r.House.Description,
                    Price = r.House.Price,
                    IsModerated = r.House.IsModerated,
                    Address = new AddressDTO
                    {
                        Id = r.House!.AddressId,
                        Latitude = r.House!.Address!.Latitude,
                        Longitude = r.House.Address.Longitude,
                        Country = r.House.Address.Country,
                        City = r.House.Address.City,
                        FormattedAddress = r.House.Address.FormattedAddress,
                        AddressLabel = r.House.Address.AddressLabel
                    },
                    Category = new CategoryDTO
                    {
                        Id = r.House.CategoryId,
                        Name = r.House!.Category!.Name
                    },
                    User = new UserDTO
                    {
                        Id = r.House.User!.Id,
                        FirstName = r.House.User.FirstName,
                        Surname = r.House.User.Surname,
                        Email = r.House.User.Email
                    }
                }
            })
            .ToListAsync();

            return rents;
        }

        //GET: api/Rents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RentDTO>> GetRent(int id)
        {
            RentDTO? rent = await _context.Rents
            .Include(r => r.House)
                .ThenInclude(h => h!.Address)
            .Include(r => r.House)
                .ThenInclude(h => h!.Category)
            .Include(r => r.User)
            .Where(r => r.Id == id)
            .Select(r => new RentDTO
            {
                Id = r.Id,
                CountOfDay = r.CountOfDay,
                Price = r.Price,
                From = r.From,
                To = r.To,
                User = new UserDTO
                {
                    Id = r.User!.Id,
                    FirstName = r.User!.FirstName,
                    Surname = r.User!.Surname,
                    Email = r.User.Email
                },
                House = new HouseDTO
                {
                    Id = r.House!.Id,
                    Description = r.House.Description,
                    Price = r.House.Price,
                    IsModerated = r.House.IsModerated,
                    Address = new AddressDTO
                    {
                        Id = r.House!.AddressId,
                        Latitude = r.House!.Address!.Latitude,
                        Longitude = r.House.Address.Longitude,
                        Country = r.House.Address.Country,
                        City = r.House.Address.City,
                        FormattedAddress = r.House.Address.FormattedAddress,
                        AddressLabel = r.House.Address.AddressLabel
                    },
                    Category = new CategoryDTO
                    {
                        Id = r.House.CategoryId,
                        Name = r.House!.Category!.Name
                    },
                    User = new UserDTO
                    {
                        Id = r.House.User!.Id,
                        FirstName = r.House.User.FirstName,
                        Surname = r.House.User.Surname,
                        Email = r.House.User.Email
                    }
                }
            })
            .FirstOrDefaultAsync();

            if (rent == null)
            {
                return NotFound();
            }

            return rent;
        }

        //POST: api/Rents
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostRent([FromBody] RentCreationDTO rentCreationDTO)
        {
            if (ModelState.IsValid)
            {
                var houseExists = await _context.Houses.AnyAsync(h => h.Id == rentCreationDTO.houseId);
                //var userExists = await _context.Users.AnyAsync(u => u.Id == rentCreationDTO.UserId);

                if (!houseExists)
                {
                    return BadRequest(new Error("Invalid HouseId."));
                }

                // Create a new Rent from the DTO
                var user = await UserFunctions.GetUser(userManager, User);
                //var house = await _context.Houses.FindAsync(rentCreationDTO.HouseId);
                //if (user == null)
                //{
                //    return NotFound($"No user found with ID {rentCreationDTO.UserId}.");
                //}
                if(rentCreationDTO.countOfDay==null) rentCreationDTO.countOfDay = 0;
                var rent = new Rent
                {
                    HouseId = rentCreationDTO.houseId,
                    CountOfDay = (int)rentCreationDTO.countOfDay,
                    Price = rentCreationDTO.price,
                    From = rentCreationDTO.from,
                    To = rentCreationDTO.to,
                    UserId = user.Id
                };

                _context.Rents.Add(rent);
                await _context.SaveChangesAsync();

                return Ok(new { rent.Id });
            }
            return BadRequest(ModelState);
        }

        //PUT: api/Rents/5
        [HttpPut("{id}")]
        public async Task<ActionResult<RentDTO>> PutRent(int id, [FromBody] RentUpdateDTO rentUpdateDTO)
        {
            if (ModelState.IsValid)
            {
                if (id != rentUpdateDTO.Id)
                {
                    return BadRequest("The ID in the URL does not match the ID in the provided data.");
                }

                var rent = await _context.Rents
                .Include(r => r.House)
                    .ThenInclude(h => h!.Address)
                .Include(r => r.House)
                    .ThenInclude(h => h!.Category)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

                if (rent == null)
                {
                    return NotFound($"No rent found with ID {id}.");
                }

                rent!.CountOfDay = rentUpdateDTO.CountOfDay;
                rent.Price = rentUpdateDTO.Price;
                rent.From = rentUpdateDTO.From;
                rent.To = rentUpdateDTO.To;

                try
                {
                    await _context.SaveChangesAsync();

                    var rentDto = new RentDTO
                    {
                        Id = rent.Id,
                        CountOfDay = rent.CountOfDay,
                        Price = rent.Price,
                        From = rent.From,
                        To = rent.To,
                        User = new UserDTO
                        {
                            Id = rent.User!.Id,
                            FirstName = rent.User.FirstName,
                            Surname =  rent.User.Surname,
                            Email = rent.User.Email
                        },
                        House = new HouseDTO
                        {
                            Id = rent.House!.Id,
                            Description = rent.House.Description,
                            Price = rent.House.Price,
                            IsModerated = rent.House.IsModerated,
                            Address = new AddressDTO
                            {
                                Id = rent.House.Address!.Id,
                                Latitude = rent.House!.Address.Latitude,
                                Longitude = rent.House.Address.Longitude,
                                Country = rent.House.Address.Country,
                                City = rent.House.Address.City,
                                FormattedAddress = rent.House.Address.FormattedAddress,
                                AddressLabel = rent.House.Address.AddressLabel
                            },
                            Category = new CategoryDTO
                            {
                                Id = rent.House.CategoryId,
                                Name = rent.House!.Category!.Name
                            },
                            User = new UserDTO
                            {
                                Id = rent.House.User!.Id,
                                FirstName = rent.House.User.FirstName,
                                Surname = rent.House.User.Surname,
                                Email = rent.House.User.Email
                            }

                        }
                    };

                    return Ok(rentDto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Rents.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return BadRequest(ModelState);
        }

        //DELETE: api/Rents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRent(int id)
        {
            if (ModelState.IsValid)
            {
                var rent = await _context.Rents.FindAsync(id);
                if (rent == null)
                {
                    return NotFound($"No rent found with ID {id}.");
                }

                _context.Rents.Remove(rent);
                await _context.SaveChangesAsync();

                return Ok($"Rent with ID {id} has been successfully deleted.");
            }
            return BadRequest(ModelState);
        }
    }
}
