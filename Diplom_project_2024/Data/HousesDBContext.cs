using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Diplom_project_2024.Data
{
    public class HousesDBContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Image> Images { get; set; } = default!;
        public DbSet<House> Houses { get; set; } = default!;
        public DbSet<Tag> Tags { get; set; } = default!;
        public DbSet<Rent> Rents { get; set; } = default!;
        public DbSet<Address> Addresses { get; set; } = default!;
        public DbSet<Chat> Chats { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
        public HousesDBContext(DbContextOptions<HousesDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<Country>().HasIndex(t => t.Name).IsUnique(true);
            builder.Entity<Category>().HasIndex(t => t.Name).IsUnique(true);
            builder.Entity<Tag>().HasIndex(t => t.Name).IsUnique(true);
            builder.Entity<User>().HasIndex(t => t.Email).IsUnique(true);
            base.OnModelCreating(builder);
        }
    }
}
