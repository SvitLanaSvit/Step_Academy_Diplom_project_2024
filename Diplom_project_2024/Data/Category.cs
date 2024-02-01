namespace Diplom_project_2024.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public List<House>? Houses { get; set; }

    }
}
