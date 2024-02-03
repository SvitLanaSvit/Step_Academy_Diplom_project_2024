namespace Diplom_project_2024.Models.DTOs
{
    public class RentCreationDTO
    {
        public string? UserName { get; set; }
        public int HouseId { get; set; }
        public int CountOfDay { get; set; }
        public double Price { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? UserId { get; set; }
    }
}
