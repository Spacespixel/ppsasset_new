using PPSAssetAdmin.Models;

namespace PPSAssetAdmin.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.SyUsers.Any())
            {
                return;   // DB has been seeded
            }

            var admin = new SyUser
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Default password, should be changed
                DisplayName = "Administrator",
                Role = "Admin",
                IsActive = true,
                LastLogin = DateTime.Now
            };

            context.SyUsers.Add(admin);
            context.SaveChanges();
        }
    }
}
