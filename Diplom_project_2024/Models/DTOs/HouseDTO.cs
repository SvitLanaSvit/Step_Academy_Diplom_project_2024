using Diplom_project_2024.Data;

namespace Diplom_project_2024.Models.DTOs
{
    public class HouseDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int SquareMeter { get; set; }
        public int Rooms { get; set; }
        public int SleepingPlaces { get; set; }
        public int ChildrenSleepingPlaces { get; set; }
        public AddressDTO? Address { get; set; }
        public CategoryDTO? Category { get; set; }
        public UserDTO? User { get; set; }
        public bool IsModerated { get; set; }
        public List<TagDTO>? Tags { get; set; }
        public List<ImageDTO>? Images { get; set; }
    }
}
