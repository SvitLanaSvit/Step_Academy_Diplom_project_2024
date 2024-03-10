namespace Diplom_project_2024.Services
{
    public class ErrorException
    {
        public string error { get; set; } = default!;
        public ErrorException(string error)
        {
            this.error = error;
        }
    }
}
