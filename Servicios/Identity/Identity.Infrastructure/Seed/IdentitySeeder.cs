using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IdentityDbContext db, IConfiguration config)
        {
            if (await db.Users.AnyAsync(u => u.Role == "SuperAdmin"))
                return;

            var userName = config["SeedAdmin:UserName"] ?? "admin";
            var email    = config["SeedAdmin:Email"]    ?? "admin@admin.com";
            var password = config["SeedAdmin:Password"] ?? "Admin1234!";

            var admin = new User
            {
                Id            = Guid.NewGuid(),
                UserName      = userName,
                Email         = email,
                Password      = BCrypt.Net.BCrypt.HashPassword(password),
                Role          = "SuperAdmin",
                IsActive      = true,
                IsFirstLogin  = false,
                CreatedAt     = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            db.Users.Add(admin);
            await db.SaveChangesAsync();

            Console.WriteLine($"[Seed] SuperAdmin '{userName}' creado.");
        }
    }
}
