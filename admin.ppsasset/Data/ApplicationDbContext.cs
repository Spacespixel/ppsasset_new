using Microsoft.EntityFrameworkCore;
using PPSAssetAdmin.Models;

namespace PPSAssetAdmin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<SyUser> SyUsers { get; set; }
        public DbSet<TrTransaction> Transactions { get; set; }
        public DbSet<SyProject> Projects { get; set; }
    }
}
