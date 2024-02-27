using System.ComponentModel.DataAnnotations;

namespace Diplom_project_2024.Models.DTOs
{
    public class HouseUpdateDTO
    {
        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public double? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Square Meter must be a positive number.")]
        public int? SquareMeter { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number of rooms must be a positive number.")]
        public int? Rooms { get; set; }
        public int? SleepingPlaces { get; set; }
        public int? ChildrenSleepingPlaces { get; set; }

        public int? CategoryId { get; set; }

        public bool? IsModerated { get; set; }
    }
}
