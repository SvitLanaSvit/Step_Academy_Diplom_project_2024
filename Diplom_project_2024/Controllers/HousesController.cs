using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Diplom_project_2024.Data;
using Diplom_project_2024.Functions;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Diplom_project_2024.CustomErrors;
using System.IO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Diplom_project_2024.Controllers //TODO PUT
{
    [Route("api/[controller]")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly HousesDBContext _context;
        BlobServiceClient blob;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        BlobContainerClient container;

        public HousesController(HousesDBContext context, BlobServiceClient client, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            this.blob = client;
            this.mapper = mapper;
            this.userManager = userManager;
            container = this.blob.GetBlobContainerClient("houseimages");
            container.CreateIfNotExists();
            container.SetAccessPolicy(PublicAccessType.BlobContainer);
        }
        [HttpGet("GetMainPageInfo")]
        public async Task<IActionResult> GetMainPageInfo()
        {
            var theBests = _context.Houses
                .Include(h => h.Comments)
                .Include(h => h.Address)
                .Include(h => h.Category)
                .Include(h => h.User)
                .Include(h => h.Tags)
                .Include(h => h.Images)
                .ToList()
                .OrderByDescending(t => GetHouseRating(t))
                //.OrderByDescending(t => GetNumber(t))
                .ThenByDescending(t => t.Comments == null ? 0 : t.Comments.Count())
                .Take(16)
                .Select(t => mapper.Map<HouseDTO>(t))
                .ToList();

            var mostPopulars = _context.Houses
                .Include(h => h.Comments)
                .Include(h => h.Address)
                .Include(h => h.Category)
                .Include(h => h.User)
                .Include(h => h.Tags)
                .Include(h => h.Images)
                .ToList()
                .OrderByDescending(t=> t.Rents==null?0: t.Rents.Count())
                .ThenByDescending(t => GetHouseRating(t))
                .ThenByDescending(t => t.Comments==null?0: t.Comments.Count())
                .Take(16)
                .Select(t => mapper.Map<HouseDTO>(t))
                .ToList();
            return Ok(new {theMostPopular = mostPopulars, theBest = theBests });
        }
        
        //GET: api/Houses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HouseDTO>>> GetHouses(string? address,
            string? from,
            string? to,
            int? adult,
            int? childs,
            int? infants,
            int? pets
            )
        {
            var houses = _context.Houses
                .Include(h=>h.Comments)
                .Include(h => h.Address)
                .Include(h => h.Category)
                .Include(h => h.User)
                .Include(h => h.Tags)
                .Include(h => h.Images)
                //.Where(t=>t.IsModerated==true)
                .ToList();
            if (!address.IsNullOrEmpty())
            {
                var split = address.Split(" ");
                if (split.Length >= 2)
                {
                    var country = split[0].ToLower();
                    var city = split[1].ToLower();
                    houses = houses.Where(t => t.Address.Country.ToLower().Contains(country) && t.Address.City.ToLower().Contains(city)).ToList();
                }
                if (split.Length == 1)
                {
                    var inf = split[0].ToLower();
                    var buf = houses.Where(t => t.Address.Country.ToLower().Contains(inf));
                    if (buf.Count() == 0)
                        houses = houses.Where(t => t.Address.City.ToLower().Contains(inf)).ToList();
                    else
                        houses = buf.ToList();
                }
            }
            if (!from.IsNullOrEmpty() && !to.IsNullOrEmpty())
            {
                var fromDate = DateTime.Parse(from);
                var toDate = DateTime.Parse(to);
                houses = houses.Where(t =>
                {
                    var rents = t.Rents;
                    if (rents == null)
                        return true;
                    var count = rents.Where(t =>
                    {
                        var from = DateTime.Parse(t.From);
                        var to = DateTime.Parse(t.To);
                        bool check = true;
                        if (!(from >= fromDate) && !(to <= fromDate))
                            check = false;
                        if (!(from >= toDate) && !(to <= toDate))
                            check = false;
                        else
                            check = true;
                        return check;
                    }).Count();
                    if (count == 0)
                        return true;
                    else
                        return false;
                }).ToList();
            }
            if (adult!=null&& adult!=0)
                houses = houses.Where(t => t.Beds >= adult).ToList();
            if(childs!=null && childs!=0)
                houses = houses.Where(t => t.ChildBeds >= childs).ToList();
            if(infants!=null && infants!=0)
                houses = houses.Where(t => t.BabyCribs >= infants).ToList();
            if(pets!=null && pets!=0)
                houses = houses.Where(t => t.Pets >= pets).ToList();

            var dto = houses.Select(t =>
            {
                var houseDTO = mapper.Map<HouseDTO>(t);
                if (t.Comments != null && t.Comments.Count>0)
                    houseDTO.Rating = t.Comments.Average(t => t.Rating);
                else
                    houseDTO.Rating = 0;
                return houseDTO;

            }).ToList();

            
            //.Select(h => new HouseDTO
            //{
            //    Id = h.Id,
            //    Description = h.Description,
            //    Price = h.Price,
            //    SquareMeter = h.SquareMeter,
            //    Rooms = h.Rooms,
            //    Address = new AddressDTO
            //    {
            //        Id = h.Address!.Id,
            //        Latitude = h.Address.Latitude,
            //        Longitude = h.Address.Longitude,
            //        Country = h.Address.Country,
            //        City = h.Address.City,
            //        FormattedAddress = h.Address.FormattedAddress,
            //        AddressLabel = h.Address.AddressLabel
            //    },
            //    Category = new CategoryDTO
            //    {
            //        Id = h.Category!.Id,
            //        Name = h.Category.Name
            //    },
            //    User = new UserDTO
            //    {
            //        Id = h.User!.Id,
            //        FirstName = h.User.FirstName,
            //        Surname = h.User.Surname,
            //        Email = h.User.Email
            //    },
            //    IsModerated = h.IsModerated,
            //    Tags = h.Tags!.Select(t => new TagDTO 
            //    { 
            //        Id = t.Id,
            //        Name = t.Name,
            //        ImagePath = t.ImagePath
            //    }).ToList(),
            //    Images = h.Images!.Select(img => new ImageDTO
            //    {
            //        Id = img.Id,
            //        Path = img.Path,
            //        IsMain = img.IsMain
            //    }).ToList()
            //})
            //.ToListAsync();

            //return Ok(houses);
            return Ok(dto);
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
                        FirstName = h.User.FirstName,
                        Surname = h.User.Surname,
                        Email = h.User.Email
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostHouse([FromForm] HouseCreateDTO houseCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await UserFunctions.GetUser(userManager, User);
                var category= await _context.Categories.FirstOrDefaultAsync(c => c.Name == houseCreateDTO.Category);
                //var userExists = await _context.Users.AnyAsync(u => u.Id == user.Id);

                List<Image> images = new List<Image>(); ;
                List<Tag> tags = new List<Tag>();

                if (category==null)
                {
                    return BadRequest(new Error("Invalid Category"));
                }


                if (houseCreateDTO.Address == null) return BadRequest(new Error("Address is must"));
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
                    AddressId = newAddress.Id,
                    CategoryId = category.Id,
                    UserId = user.Id,
                    Tags = tags,
                    AccomodationType = houseCreateDTO.AccomodationType,
                    BabyCribs = houseCreateDTO.BabyCribs,
                    Bathrooms = houseCreateDTO.Bathrooms,
                    Beds = houseCreateDTO.Beds,
                    ChildBeds = houseCreateDTO.ChildBeds,
                    IsModerated = false,
                    Pets= houseCreateDTO.Pets,
                    Name = houseCreateDTO.Name
                };
                //newHouse.UserId = user.Id;
                //newHouse.UserId = "20b6cce4-2194-4b28-a5fe-05d522a0c8f0";

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

                if (houseCreateDTO.Tags != null)
                {
                    foreach (var item in houseCreateDTO.Tags)
                    {
                        Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == item);
                        if (tag == null) return BadRequest(new Error("Tag wasnt found"));
                        tags.Add(tag);
                    }

                }
                await _context.SaveChangesAsync();
                var houseDTO = mapper.Map<HouseDTO>(newHouse);
                return Ok(houseDTO);
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
                    FirstName = house.User.FirstName,
                    Surname = house.User.Surname,
                    Email = house.User.Email
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
        private double GetHouseRating(House house)
        {
            if (house.Comments != null && house.Comments.Count > 0)
                return house.Comments.Average(t => t.Rating);
            else
                return 0;
        }
        private double GetNumber(House h)
        {
            return 0;
        }
    }
}
