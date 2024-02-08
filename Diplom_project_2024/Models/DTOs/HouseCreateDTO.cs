using Diplom_project_2024.Data;

namespace Diplom_project_2024.Models.DTOs
{
    public class HouseCreateDTO
    {
        public string? Description { get; set; } 
        public double Price { get; set; }
        public int SquareMeter { get; set; }
        public int Rooms { get; set; }
        public Address? Address { get; set; }
        public int CategoryId { get; set; }
        public string? UserName { get; set; }  
        public string? UserId { get; set; }

        public List<int>? TagIds { get; set; }
        public List<IFormFile>? Images { get; set; }
        public int MainImage { get; set; }
    }
}
