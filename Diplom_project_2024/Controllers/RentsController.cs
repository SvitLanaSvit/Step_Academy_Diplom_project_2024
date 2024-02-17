using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentsController : ControllerBase
    {
        private readonly HousesDBContext _context;

        public RentsController(HousesDBContext context)
        {
            _context = context;
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
                    DisplayName = r.User!.DisplayName,
                    Email = r.User.Email,
                    ImagePath = r.User.ImagePath,
                    UserName = r.User.UserName,
                    PhoneNumber = r.User.PhoneNumber
                },
                House = new HouseDTO
                {
                    Id = r.House!.Id,
                    Description = r.House.Description,
                    Price = r.House.Price,
                    SquareMeter = r.House.SquareMeter,
                    Rooms = r.House.Rooms,
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
                        DisplayName = r.House.User.DisplayName,
                        Email = r.House.User.Email,
                        ImagePath = r.House.User.ImagePath,
                        UserName = r.House.User.UserName,
                        PhoneNumber = r.House.User.PhoneNumber
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
                    DisplayName = r.User!.DisplayName,
                    Email = r.User.Email,
                    ImagePath = r.User.ImagePath,
                    UserName = r.User.UserName,
                    PhoneNumber = r.User.PhoneNumber
                },
                House = new HouseDTO
                {
                    Id = r.House!.Id,
                    Description = r.House.Description,
                    Price = r.House.Price,
                    SquareMeter = r.House.SquareMeter,
                    Rooms = r.House.Rooms,
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
                        DisplayName = r.House.User.DisplayName,
                        Email = r.House.User.Email,
                        ImagePath = r.House.User.ImagePath,
                        UserName = r.House.User.UserName,
                        PhoneNumber = r.House.User.PhoneNumber
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
        [HttpPost]
        public async Task<IActionResult> PostRent([FromBody] RentCreationDTO rentCreationDTO)
        {
            if (ModelState.IsValid)
            {
                var houseExists = await _context.Houses.AnyAsync(h => h.Id == rentCreationDTO.HouseId);
                var userExists = await _context.Users.AnyAsync(u => u.Id == rentCreationDTO.UserId);

                if (!houseExists || !userExists)
                {
                    return BadRequest("Invalid HouseId or UserId.");
                }

                // Create a new Rent from the DTO
                var user = await _context.Users.FindAsync(rentCreationDTO.UserId);
                //var house = await _context.Houses.FindAsync(rentCreationDTO.HouseId);
                if (user == null)
                {
                    return NotFound($"No user found with ID {rentCreationDTO.UserId}.");
                }
                var rent = new Rent
                {
                    HouseId = rentCreationDTO.HouseId,
                    CountOfDay = rentCreationDTO.CountOfDay,
                    Price = rentCreationDTO.Price,
                    From = rentCreationDTO.From,
                    To = rentCreationDTO.To,
                    User = user
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
                            DisplayName = rent.User.DisplayName,
                            Email = rent.User.Email,
                            ImagePath = rent.User.ImagePath,
                            UserName = rent.User.UserName,
                            PhoneNumber = rent.User.PhoneNumber
                        },
                        House = new HouseDTO
                        {
                            Id = rent.House!.Id,
                            Description = rent.House.Description,
                            Price = rent.House.Price,
                            SquareMeter = rent.House.SquareMeter,
                            Rooms = rent.House.Rooms,
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
                                DisplayName = rent.House.User.DisplayName,
                                Email = rent.House.User.Email,
                                ImagePath = rent.House.User.ImagePath,
                                UserName = rent.House.User.UserName,
                                PhoneNumber = rent.House.User.PhoneNumber
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
