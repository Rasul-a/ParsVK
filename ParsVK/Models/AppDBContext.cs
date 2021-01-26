using Microsoft.EntityFrameworkCore;

namespace ParsVK.Models
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> opt):base(opt)
        {
           // Database.EnsureCreated();
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<WallItem> WallItems { get; set; }
        public DbSet<LikeUser> LikeUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LikeUser>()
                .HasOne(p => p.Profile)
                .WithMany(t => t.LikeUsers)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WallItem>()
                .HasOne(p => p.Profile)
                .WithMany(t => t.WallItems)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
