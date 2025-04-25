using Microsoft.EntityFrameworkCore;
using Identity.Domain.Entities;

namespace Identity.Infrastructure
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Restaurante> Restaurantes => Set<Restaurante>();

        public DbSet<UserRestaurante> UserRestaurantes => Set<UserRestaurante>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<UserRestaurante>()
             .HasKey(ur => new { ur.UserId, ur.RestaurantId });

            b.Entity<UserRestaurante>()
             .HasOne(ur => ur.User)
             .WithMany(u => u.Restaurantes)               // o con navegación UsersRestaurants si la tienes
             .HasForeignKey(ur => ur.UserId);

            b.Entity<UserRestaurante>()
             .HasOne(ur => ur.Restaurante)
             .WithMany(r => r.Usuarios)
             .HasForeignKey(ur => ur.RestaurantId);

            b.Entity<Restaurante>()
             .HasIndex(r => r.Slug)
             .IsUnique();
        }
    }
}
