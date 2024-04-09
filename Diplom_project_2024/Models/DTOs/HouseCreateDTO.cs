using Diplom_project_2024.Data;

namespace Diplom_project_2024.Models.DTOs
{
    public class HouseCreateDTO
    {
        public string Category { get; set; } = default!;
        public string AccomodationType { get; set; } = default!;
        public AddressCreateDTO? Address { get; set; }
        public int Beds { get; set; }
        public int ChildBeds { get; set; }
        public int BabyCribs { get; set; }
        public int Pets { get; set; }
        public int Bathrooms { get; set; }
        public List<string>? Tags { get; set; }
        public List<IFormFile>? Images { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public double Price { get; set; }
        
        public int MainImage { get; set; }
    }
}
