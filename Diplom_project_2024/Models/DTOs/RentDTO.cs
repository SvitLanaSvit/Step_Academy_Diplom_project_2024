namespace Diplom_project_2024.Models.DTOs
{
    public class RentDTO
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public int HouseId { get; set; }
        public int CountOfDay { get; set; }
        public double? Price { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public UserDTO? User { get; set; }
        public HouseDTO? House { get; set; }
    }
}
