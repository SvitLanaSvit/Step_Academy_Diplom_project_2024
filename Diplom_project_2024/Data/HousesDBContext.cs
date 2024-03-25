using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        public DbSet<PaymentData> PaymentDatas { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<FavoriteHouse> FavoriteHouses { get; set; } = default!;
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
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Comment>()
                .HasOne(c => c.House)
                .WithMany(h => h.Comments)
                .HasForeignKey(c => c.HouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteHouse>()
                .HasOne(fh => fh.User)
                .WithMany(u => u.FavoriteHouses)
                .HasForeignKey(fh => fh.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteHouse>()
                .HasOne(fh => fh.House)
                .WithMany(h => h.Fans)
                .HasForeignKey(fh => fh.HouseId)
                .OnDelete(DeleteBehavior.NoAction);
            base.OnModelCreating(builder);
        }
    }
}
