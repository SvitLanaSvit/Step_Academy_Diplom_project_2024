using Diplom_project_2024.Data;

namespace Diplom_project_2024.Models.DTOs
{
    public class HouseDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string Name { get; set; } = default!;
        public int Beds { get; set; }
        public int ChildBeds { get; set; }
        public int BabyCribs { get; set; }
        public int Pets { get; set; }
        public int Bathrooms { get; set; }
        public AddressDTO? Address { get; set; }
        public CategoryDTO? Category { get; set; }
        public UserDTO? User { get; set; }
        public bool IsModerated { get; set; }
        public List<TagDTO>? Tags { get; set; }
        public List<ImageDTO>? Images { get; set; }
        public double Rating { get; set; }
    }
}
