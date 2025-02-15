using SoundScape.Data;
using SoundScape.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundScape.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ApplicationDbContext _context;

        public PlaylistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Отримання всіх плейлистів
        public async Task<List<Playlist>> GetAllPlaylistsAsync()
        {
            return await _context.Playlists.ToListAsync();
        }

        // Отримання плейлиста за id
        public async Task<Playlist> GetPlaylistByIdAsync(int id)
        {
            return await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Створення нового плейлиста
        public async Task<bool> CreatePlaylistAsync(Playlist playlist)
        {
            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
            return true;
        }

        // Додавання пісні до плейлиста
        public async Task<bool> AddSongToPlaylistAsync(int playlistId, int songId)
        {
            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId
            };

            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();
            return true;
        }

        // Видалення пісні з плейлиста
        public async Task<bool> RemoveSongFromPlaylistAsync(int playlistId, int songId)
        {
            var playlistSong = await _context.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistSong == null)
                return false;

            _context.PlaylistSongs.Remove(playlistSong);
            await _context.SaveChangesAsync();
            return true;
        }

        // Видалення плейлиста
        public async Task<bool> DeletePlaylistAsync(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
                return false;

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
