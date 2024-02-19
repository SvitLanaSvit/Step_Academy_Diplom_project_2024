using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Diplom_project_2024.Data;
using Diplom_project_2024.Functions;
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
            if (ModelState.IsValid)
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
                        // Перевірка на індекс основного зображення
                        bool isMain = houseCreateDTO.Images.IndexOf(image) == houseCreateDTO.MainImage;

                        //string filename = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        //BlobClient client = container.GetBlobClient(filename);
                        //await client.UploadAsync(image.OpenReadStream());

                        // Використання статичного метода для завантаження зображення
                        string imagePath = await BlobContainerFunctions.UploadImage(container, image);

                        // Create a new Image entity and link it to the house
                        var houseImage = new Image
                        {
                            House = newHouse,
                            HouseId = newHouse.Id,
                            IsMain = isMain,
                            //Path = client.Uri.AbsoluteUri
                            Path = imagePath
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
            return BadRequest(ModelState);
        }

        //PUT: api/Houses/5
        [HttpPut("{id}")]
        public async Task<ActionResult<HouseDTO>> PutHouse(int id, [FromBody] HouseUpdateDTO houseUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var house = await _context.Houses
                .Include(h => h.Address)
                .Include(h => h.Category)
                .Include(h => h.User)
                .Include(h => h.Tags)
                .Include(h => h.Images)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return NotFound($"House with ID {id} not found.");
            }

            // Оновлення полів об'єкта нерухомості
            house.Description = houseUpdateDTO.Description ?? house.Description;
            house.Price = houseUpdateDTO.Price ?? house.Price;
            house.SquareMeter = houseUpdateDTO.SquareMeter ?? house.SquareMeter;
            house.Rooms = houseUpdateDTO.Rooms ?? house.Rooms;

            if (houseUpdateDTO.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == houseUpdateDTO.CategoryId.Value);
                if (!categoryExists)
                {
                    return BadRequest($"Category with ID {houseUpdateDTO.CategoryId.Value} does not exist.");
                }
                house.CategoryId = houseUpdateDTO.CategoryId.Value;
            }

            if (houseUpdateDTO.IsModerated.HasValue)
            {
                house.IsModerated = houseUpdateDTO.IsModerated.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Створення та повернення HouseDTO
            var houseDTO = new HouseDTO
            {
                Id = house.Id,
                Description = house.Description,
                Price = house.Price,
                SquareMeter = house.SquareMeter,
                Rooms = house.Rooms,
                IsModerated = house.IsModerated,
                Address = new AddressDTO
                {
                    Id = house.Address!.Id,
                    Longitude = house.Address.Longitude,
                    Latitude = house.Address.Latitude,
                    Country = house.Address.Country,
                    City = house.Address.City,
                    FormattedAddress = house.Address.FormattedAddress,
                    AddressLabel = house.Address.AddressLabel
                },
                Category = new CategoryDTO
                {
                    Id = house.Category!.Id,
                    Name = house.Category.Name
                },
                User = new UserDTO
                {
                    Id = house.User!.Id,
                    DisplayName = house.User.DisplayName,
                    Email = house.User.Email,
                    ImagePath = house.User.ImagePath,
                    UserName = house.User.UserName,
                    PhoneNumber = house.User.PhoneNumber
                },
                Tags = house.Tags?.Select(t => new TagDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    ImagePath = t.ImagePath
                }).ToList(),
                Images = house.Images?.Select(i => new ImageDTO
                {
                    Id = i.Id,
                    Path = i.Path,
                    IsMain = i.IsMain
                }).ToList()
            };

            return Ok(houseDTO);
        }

        //DELETE: api/Houses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHouse(int id)
        {
            if (ModelState.IsValid)
            {
                var house = await _context.Houses
                              .Include(h => h.Images)
                              .SingleOrDefaultAsync(h => h.Id == id);

                if (house == null)
                {
                    return NotFound($"House with ID {id} not found.");
                }

                // Видалення зображень з Azure Blob Storage
                foreach (var image in house.Images!)
                {
                    BlobContainerFunctions.DeleteImage(container, image.Path);
                }

                // Видалення пов'язаної адреси
                var address = await _context.Addresses.FindAsync(house.AddressId);
                if (address != null)
                {
                    _context.Addresses.Remove(address);
                }

                _context.Houses.Remove(house);
                await _context.SaveChangesAsync();

                return Ok($"House with ID {id} and associated resources have been successfully deleted.");
            }
            return BadRequest(ModelState);
        }

        private bool HouseExists(int id)
        {
            return _context.Houses.Any(e => e.Id == id);
        }
    }
}
