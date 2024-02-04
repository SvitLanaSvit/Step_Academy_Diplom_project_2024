namespace Diplom_project_2024.Models.DTOs
{
    public class HouseDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int SquareMeter { get; set; }
        public int Rooms { get; set; }
        public AddressDTO? Address { get; set; }
        public CategoryDTO? Category { get; set; }
        public UserDTO? User { get; set; }
        public bool IsModerated { get; set; }
    }
}
