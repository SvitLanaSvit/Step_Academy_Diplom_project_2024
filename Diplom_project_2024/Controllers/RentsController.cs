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
                Username = r.Username,
                HouseId = r.HouseId,
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
                    Username = r.House.Username,
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
                Username = r.Username,
                HouseId = r.HouseId,
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
                    Username = r.House.Username,
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
                    }
                }
            })
            .FirstOrDefaultAsync();

            if(rent == null)
            {
                return NotFound();
            }

            return rent;
        }

        //POST: api/Rents
        [HttpPost]
        public async Task<IActionResult> PostRent(RentCreationDTO rentCreationDTO)
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
            var rent = new Rent
            {
                Username = rentCreationDTO.UserName,
                HouseId = rentCreationDTO.HouseId,
                CountOfDay = rentCreationDTO.CountOfDay,
                Price = rentCreationDTO.Price,
                From = rentCreationDTO.From,
                To = rentCreationDTO.To,
                User = user
            };

            _context.Rents.Add(rent);
            await _context.SaveChangesAsync();

            return Ok(new {rent.Id});
        }
    }
}
