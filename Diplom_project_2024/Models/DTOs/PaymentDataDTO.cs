namespace Diplom_project_2024.Models.DTOs
{
    public class PaymentDataDTO
    {
        public string CardNumber { get; set; } = default!;
        public DateOnly ExpireDate { get; set; }
        public string CVV { get; set; } = default!;
    }
}
