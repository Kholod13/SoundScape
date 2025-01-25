using SoundScape.Data;
using SoundScape.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SoundScape.Services
{
    public class SongService : ISongService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SongService> _logger;

        public SongService(ApplicationDbContext context, ILogger<SongService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Song>> GetAllSongsAsync()
        {
            try
            {
                return await _context.Songs.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching songs from the database.");
                throw; // Перепрограмування помилки після логування
            }
        }

        public async Task<Song> GetSongByIdAsync(int id)
        {
            try
            {
                return await _context.Songs.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching song with id {id} from the database.");
                throw;
            }
        }

        public async Task AddSongAsync(Song song)
        {
            try
            {
                // Перевірка на наявність пісні (якщо потрібно)
                var existingSong = await _context.Songs
                    .FirstOrDefaultAsync(s => s.Title == song.Title && s.Artist == song.Artist);
                if (existingSong != null)
                {
                    _logger.LogWarning($"Song '{song.Title}' by '{song.Artist}' already exists.");
                    return;
                }

                _context.Songs.Add(song);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Song '{song.Title}' by '{song.Artist}' added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding song to the database.");
                throw;
            }
        }

        public async Task UpdateSongAsync(Song song)
        {
            try
            {
                var existingSong = await _context.Songs.FindAsync(song.Id);
                if (existingSong == null)
                {
                    _logger.LogWarning($"Song with id {song.Id} not found.");
                    return;
                }

                _context.Songs.Update(song);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Song '{song.Title}' by '{song.Artist}' updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating song with id {song.Id}.");
                throw;
            }
        }

        public async Task DeleteSongAsync(int id)
        {
            try
            {
                var song = await _context.Songs.FindAsync(id);
                if (song == null)
                {
                    _logger.LogWarning($"Song with id {id} not found.");
                    return;
                }

                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Song with id {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting song with id {id}.");
                throw;
            }
        }
    }
}
