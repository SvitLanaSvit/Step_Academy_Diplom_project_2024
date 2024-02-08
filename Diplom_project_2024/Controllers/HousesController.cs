using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

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
                .Include(h => h.Tags)
                .Include(h => h.Images)
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
                    IsModerated = h.IsModerated,
                    Tags = h.Tags!.Select(t => new TagDTO 
                    { 
                        Id = t.Id,
                        Name = t.Name,
                        ImagePath = t.ImagePath
                    }).ToList(),
                    Images = h.Images!.Select(img => new ImageDTO
                    {
                        Id = img.Id,
                        Path = img.Path,
                        IsMain = img.IsMain
                    }).ToList()
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
                    IsModerated = h.IsModerated,
                    Tags = h.Tags!.Select(t => new TagDTO
                    {
                        Id = t.Id,
                        Name = t.Name,
                        ImagePath = t.ImagePath
                    }).ToList(),
                    Images = h.Images!.Select(img => new ImageDTO
                    {
                        Id = img.Id,
                        Path = img.Path,
                        IsMain = img.IsMain
                    }).ToList()
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
        public async Task<IActionResult> PostHouse([FromForm] HouseCreateDTO houseCreateDTO)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == houseCreateDTO.CategoryId);
            var userExists = await _context.Users.AnyAsync(u => u.Id == houseCreateDTO.UserId);

            List<Image> images = new List<Image>(); ;
            List<Tag> tags = new List<Tag>();

            if (!categoryExists || !userExists)
            {
                return BadRequest("Invalid CategoryId or UserId.");
            }

            var newAddress = new Address
            {
                // Assuming your Address object has these properties. Adjust accordingly.
                Latitude = houseCreateDTO.Address!.Latitude,
                Longitude = houseCreateDTO.Address.Longitude,
                Country = houseCreateDTO.Address.Country,
                City = houseCreateDTO.Address.City,
                FormattedAddress = houseCreateDTO.Address.FormattedAddress,
                AddressLabel = houseCreateDTO.Address.AddressLabel
            };
            _context.Addresses.Add(newAddress);
            await _context.SaveChangesAsync();

            // Create the house entity
            var newHouse = new House
            {
                Description = houseCreateDTO.Description!,
                Price = houseCreateDTO.Price,
                SquareMeter = houseCreateDTO.SquareMeter,
                Rooms = houseCreateDTO.Rooms,
                AddressId = newAddress.Id,
                CategoryId = houseCreateDTO.CategoryId,
                UserId = houseCreateDTO.UserId!,
                Tags = tags
            };

            _context.Houses.Add(newHouse);
            await _context.SaveChangesAsync();

            // Handle image uploads if any
            if (houseCreateDTO.Images != null)
            {
                foreach (var image in houseCreateDTO.Images)
                {
                    bool isMain = houseCreateDTO.Images.IndexOf(image) == houseCreateDTO.MainImage;

                    string filename = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    BlobClient client = container.GetBlobClient(filename);
                    await client.UploadAsync(image.OpenReadStream());

                    // Create a new Image entity and link it to the house
                    var houseImage = new Image
                    {
                        House = newHouse,
                        HouseId = newHouse.Id,
                        IsMain = isMain,
                        Path = client.Uri.AbsoluteUri
                    };

                    images.Add(houseImage);
                }
                await _context.AddRangeAsync(images);
                await _context.SaveChangesAsync();
            }

            if (houseCreateDTO.TagIds != null)
            {
                foreach (var item in houseCreateDTO.TagIds)
                {
                    Tag tag = await _context.Tags.FirstAsync(t => t.Id == item);
                    tags.Add(tag);
                }

            }
            await _context.SaveChangesAsync();

            return Ok(new { newHouse.Id });
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
