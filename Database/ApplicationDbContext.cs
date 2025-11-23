using Microsoft.EntityFrameworkCore;
using SaboreIA.Models;

namespace SaboreIA.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.Role)
                    .HasConversion<string>();

                // Relacionamento User -> Restaurants (1:N)
                entity.HasMany(u => u.Restaurants)
                    .WithOne(r => r.Owner)
                    .HasForeignKey(r => r.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Usuário -> Endereço (1:1)
                modelBuilder.Entity<User>()
                    .HasOne(u => u.Address)
                    .WithOne(a => a.User)
                    .HasForeignKey<User>(u => u.AddressId)
                    .OnDelete(DeleteBehavior.SetNull); // Usuário pode não ter endereço
            });

            // Restaurant entity configuration
            modelBuilder.Entity<Restaurant>(entity =>
            {
                // Relacionamento Restaurant -> Owner (N:1)
                entity.HasOne(r => r.Owner)
                    .WithMany(u => u.Restaurants)
                    .HasForeignKey(r => r.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento Restaurant -> Address (N:1)
                entity.HasOne(r => r.Address)
                    .WithMany(a => a.Restaurants)
                    .HasForeignKey(r => r.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento Restaurant <-> Tag (N:M)
                entity.HasMany(r => r.Categories)
                    .WithMany(c => c.Restaurants)
                    .UsingEntity(j => j.ToTable("RestaurantTag"));
            });

            // Address entity configuration
            modelBuilder.Entity<Address>(entity =>
            {
                // Relacionamento Address -> Restaurants (1:N)
                entity.HasMany(a => a.Restaurants)
                    .WithOne(r => r.Address)
                    .HasForeignKey(r => r.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Tag entity configuration
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();

                // Relacionamento Tag <-> Restaurant (N:M)
                entity.HasMany(c => c.Restaurants)
                    .WithMany(r => r.Categories)
                    .UsingEntity(j => j.ToTable("RestaurantTag"));
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // Relacionamento Product -> Tag (N:1)
                entity.HasOne(p => p.Tag)
                    .WithMany(rc => rc.Products)
                    .HasForeignKey(p => p.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Review entity configuration
            modelBuilder.Entity<Review>(entity =>
            {
                // Relacionamento Review -> User (N:1)
                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento Review -> Restaurant (N:1)
                entity.HasOne(r => r.Restaurant)
                    .WithMany()
                    .HasForeignKey(r => r.RestaurantId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índice único para evitar múltiplas reviews do mesmo usuário para o mesmo restaurante
                entity.HasIndex(r => new { r.UserId, r.RestaurantId }).IsUnique();
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                // Relacionamento Favorite -> User (N:1)
                entity.HasOne(f => f.User)
                    .WithMany()
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento Favorite -> Restaurant (N:1)
                entity.HasOne(f => f.Restaurant)
                    .WithMany()
                    .HasForeignKey(f => f.RestaurantId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índice único para evitar múltiplos favoritos do mesmo usuário para o mesmo restaurante
                entity.HasIndex(f => new { f.UserId, f.RestaurantId }).IsUnique();
            });
        }
    }
}
