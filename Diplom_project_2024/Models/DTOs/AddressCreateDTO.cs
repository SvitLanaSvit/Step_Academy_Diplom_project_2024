namespace Diplom_project_2024.Models.DTOs
{
    public class AddressCreateDTO
    {
        public string Latitude { get; set; } = default!;
        public string Longitude { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string City { get; set; } = default!;
        public string FormattedAddress { get; set; } = default!;
        public string AddressLabel { get; set; } = default!;
    }
}
