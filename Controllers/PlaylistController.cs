using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/playlists")] // Виправлено маршрут на "playlists"
    public class PlaylistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlaylistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/playlists
        [HttpGet]
        public async Task<IActionResult> GetAllPlaylists()
        {
            var playlists = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song) // Завантажуємо пісні
                .ToListAsync();

            var result = playlists.Select(playlist => new
            {
                playlist.Id,
                playlist.Name,
                Songs = playlist.PlaylistSongs.Select(ps => new
                {
                    ps.Song.Id,
                    ps.Song.Title,  // Використовуємо Title замість Name
                    ps.Song.Artist,
                    ps.Song.Album,
                    ps.Song.Duration,
                    ps.Song.FilePath
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/playlists/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylistWithSongs(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song) // Завантажуємо пісні
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return NotFound(new { message = "Playlist not found" });
            }

            var result = new
            {
                playlist.Id,
                playlist.Name,
                Songs = playlist.PlaylistSongs.Select(ps => new
                {
                    ps.Song.Id,
                    ps.Song.Title,  // Використовуємо Title замість Name
                    ps.Song.Artist,
                    ps.Song.Album,
                    ps.Song.Duration,
                    ps.Song.FilePath
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/playlists
        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] Playlist playlist)
        {
            if (playlist == null || string.IsNullOrWhiteSpace(playlist.Name))
            {
                return BadRequest(new { message = "Invalid playlist data." });
            }

            playlist.PlaylistSongs = new List<PlaylistSong>(); // Ініціалізуємо список

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylistWithSongs), new { id = playlist.Id }, playlist);
        }

        // DELETE: api/playlists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);

            if (playlist == null)
            {
                return NotFound(new { message = "Playlist not found" });
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/playlists/{playlistId}/songs/{songId}
        [HttpPost("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, int songId)
        {
            var playlist = await _context.Playlists.Include(p => p.PlaylistSongs).FirstOrDefaultAsync(p => p.Id == playlistId);
            var song = await _context.Songs.FindAsync(songId);

            if (playlist == null || song == null)
            {
                return NotFound(new { message = "Playlist or Song not found" });
            }

            if (playlist.PlaylistSongs == null)
            {
                playlist.PlaylistSongs = new List<PlaylistSong>();
            }

            if (playlist.PlaylistSongs.Any(ps => ps.SongId == songId))
            {
                return BadRequest(new { message = "The song is already in the playlist." });
            }

            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId
            };

            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Song added to playlist successfully" });
        }

        // DELETE: api/playlists/{playlistId}/songs/{songId}
        [HttpDelete("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var playlistSong = await _context.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistSong == null)
            {
                return NotFound(new { message = "Song not found in the playlist" });
            }

            _context.PlaylistSongs.Remove(playlistSong);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
