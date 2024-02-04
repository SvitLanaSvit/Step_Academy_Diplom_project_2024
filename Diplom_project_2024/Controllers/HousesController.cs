using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom_project_2024.Controllers //TODO PUT
{
    [Route("api/[controller]")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly HousesDBContext _context;
        BlobServiceClient blob;
        BlobContainerClient container;

        public HousesController(HousesDBContext context, BlobServiceClient client)
        {
            _context = context;
            this.blob = client;
            container = this.blob.GetBlobContainerClient("houseimages");
            container.CreateIfNotExists();
            container.SetAccessPolicy(PublicAccessType.BlobContainer);
        }

        //GET: api/Houses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HouseDTO>>> GetHouses()
        {
            List<HouseDTO> houses = await _context.Houses
                .Include(h => h.Address)
                .Include(h => h.Category)
                .Include(h => h.User)
                .Select(h => new HouseDTO
                {
                    Id = h.Id,
                    Description = h.Description,
                    Price = h.Price,
                    SquareMeter = h.SquareMeter,
                    Rooms = h.Rooms,
                    Address = new AddressDTO
                    {
                        Id = h.Address!.Id,
                        Latitude = h.Address.Latitude,
                        Longitude = h.Address.Longitude,
                        Country = h.Address.Country,
                        City = h.Address.City,
                        FormattedAddress = h.Address.FormattedAddress,
                        AddressLabel = h.Address.AddressLabel
                    },
                    Category = new CategoryDTO
                    {
                        Id = h.Category!.Id,
                        Name = h.Category.Name
                    },
                    User = new UserDTO
                    {
                        Id = h.User!.Id,
                        DisplayName = h.User.DisplayName,
                        Email = h.User.Email,
                        ImagePath = h.User.ImagePath,
                        UserName = h.User.UserName,
                        PhoneNumber = h.User.PhoneNumber
                    },
                    IsModerated = h.IsModerated
                })
                .ToListAsync();

            return Ok(houses);
        }

        //GET: api/Houses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HouseDTO>> GetHouse(int id) 
        {
            var house = await _context.Houses
                .Include(h => h.Address)
                .Include(h => h.Category)
                .Include(h => h.User)
                .Where(h => h.Id == id)
                .Select(h => new HouseDTO
                {
                    Id = h.Id,
                    Description = h.Description,
                    Price = h.Price,
                    SquareMeter = h.SquareMeter,
                    Rooms = h.Rooms,
                    Address = new AddressDTO
                    {
                        Id = h.Address!.Id,
                        Latitude = h.Address.Latitude,
                        Longitude = h.Address.Longitude,
                        Country = h.Address.Country,
                        City = h.Address.City,
                        FormattedAddress = h.Address.FormattedAddress,
                        AddressLabel = h.Address.AddressLabel
                    },
                    Category = new CategoryDTO
                    {
                        Id = h.Category!.Id,
                        Name = h.Category.Name
                    },
                    User = new UserDTO
                    {
                        Id = h.User!.Id,
                        DisplayName = h.User.DisplayName,
                        Email = h.User.Email,
                        ImagePath = h.User.ImagePath,
                        UserName = h.User.UserName,
                        PhoneNumber = h.User.PhoneNumber
                    },
                    IsModerated = h.IsModerated
                })
                .FirstOrDefaultAsync();

            if (house == null)
            {
                return NotFound();
            }

            return Ok(house);
        }

        //Post: api/Houses
        [HttpPost]
        public async Task<IActionResult> PostHouse([FromBody] HouseCreateDTO houseCreateDTO)
        {
            //var addressExists = await _context.Addresses.AnyAsync(a => a.Id == houseCreateDTO.AddressId);
            //var categoryExists = await _context.Categories.AnyAsync(c => c.Id == houseCreateDTO.CategoryId);
            //var userExists = await _context.Users.AnyAsync(u => u.Id == houseCreateDTO.UserId);
            //var tagsExists = await _context.Tags.AnyAsync(); //TODO

            //if(!addressExists || !categoryExists || !userExists) 
            //{
            //    return BadRequest("Invalid AddressId or CategoryId or UserId.");
            //}

            //var address = await _context.Addresses.FindAsync(houseCreateDTO.AddressId);
            //var category = await _context.Categories.FindAsync(houseCreateDTO.CategoryId);
            //var user = await _context.Users.FindAsync(houseCreateDTO.UserId);
            //var tags = _context.Tags.Where(t => houseCreateDTO.TagIds!.Contains(t.Id)).ToList();

            //if(address == null)
            //{
            //    return NotFound($"No address found with ID {houseCreateDTO.AddressId}.");
            //}

            //if(category == null)
            //{
            //    return NotFound($"No category found with ID {houseCreateDTO.CategoryId}.");
            //}

            //if(user == null)
            //{
            //    NotFound($"No user found with ID {houseCreateDTO.UserId}.");
            //}

            //var house = new House
            //{
            //    Description = houseCreateDTO.Description,
            //    Price = houseCreateDTO.Price,
            //    SquareMeter = houseCreateDTO.SquareMeter,
            //    Rooms = houseCreateDTO.Rooms,
            //    IsModerated = houseCreateDTO.IsModerated,
            //    Address = address,
            //    Category = category,
            //    User = user,
            //    Tags = tags
            //};

            //_context.Houses.Add(house);
            //await _context.SaveChangesAsync();

            //return Ok(new { house.Id });
            return NoContent();
        }

        //PUT: api/Houses/5
        [HttpPut("{id}")]
        public async Task<ActionResult<HouseDTO>> PutHouse(int id, [FromBody] HouseUpdateDTO houseUpdateDTO)
        {
            return NoContent();
        }

        //DELETE: api/Houses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHouse(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();

            return Ok($"House with ID {id} has been successfully deleted.");
        }
    }
}
