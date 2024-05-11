namespace Diplom_project_2024.Models.DTOs
{
    public class RentUpdateDTO
    {
        public int Id { get; set; }
        public int CountOfDay { get; set; }
        public double Price { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
    }
}
