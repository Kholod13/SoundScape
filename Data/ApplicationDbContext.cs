using Microsoft.EntityFrameworkCore;
using SoundScape.Models;

namespace SoundScape.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }
    }
}
