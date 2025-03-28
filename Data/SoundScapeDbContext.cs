using Microsoft.EntityFrameworkCore;
using SoundScape.Models;  // Додайте цей using

namespace SoundScape.Data
{
    public class SoundScapeDbContext : DbContext
    {
        public SoundScapeDbContext(DbContextOptions<SoundScapeDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Можна додавати додаткові налаштування моделей тут, якщо потрібно
        }
    }
}
