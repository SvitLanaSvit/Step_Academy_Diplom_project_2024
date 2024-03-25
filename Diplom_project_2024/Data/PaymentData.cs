namespace Diplom_project_2024.Data
{
    public class PaymentData
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public User? User { get; set; } 
        public string CardNumber { get; set; } = default!;
        public DateOnly ExpireDate { get; set; }
        public string CVV { get; set; } = default!;
    }
}
