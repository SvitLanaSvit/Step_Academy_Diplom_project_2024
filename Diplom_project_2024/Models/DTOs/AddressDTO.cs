namespace Diplom_project_2024.Models.DTOs
{
    public class AddressDTO
    {
        public int Id { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? FormattedAddress { get; set; }
        public string? AddressLabel { get; set; }
    }
}
