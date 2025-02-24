using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.DTOs;
using SoundScape.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/playlists")]
    public class PlaylistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlaylistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Отримати всі плейлисти
        [HttpGet]
        public async Task<IActionResult> GetAllPlaylists()
        {
            var playlists = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .ToListAsync();

            var playlistDTOs = playlists.Select(p => new PlaylistDTO
            {
                Id = p.Id,
                Name = p.Name,
                SongIds = p.PlaylistSongs.Select(ps => ps.SongId).ToList() // Отримуємо список ідентифікаторів пісень
            }).ToList();

            return Ok(playlistDTOs);
        }

        // Отримати плейлист за ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylistById(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
                return NotFound();

            var playlistDTO = new PlaylistDTO
            {
                Id = playlist.Id,
                Name = playlist.Name,
                SongIds = playlist.PlaylistSongs.Select(ps => ps.SongId).ToList() // Отримуємо список ідентифікаторів пісень
            };

            return Ok(playlistDTO);
        }

        // Створити новий плейлист
        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistDTO playlistDTO)
        {
            if (playlistDTO == null || string.IsNullOrWhiteSpace(playlistDTO.Name))
                return BadRequest();

            var playlist = new Playlist
            {
                Name = playlistDTO.Name
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            // Додаємо пісні до плейлиста
            foreach (var songId in playlistDTO.SongIds)
            {
                _context.PlaylistSongs.Add(new PlaylistSong
                {
                    PlaylistId = playlist.Id,
                    SongId = songId
                });
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylistById), new { id = playlist.Id }, playlist);
        }

        // Оновити інформацію про плейлист
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlaylist(int id, [FromBody] PlaylistDTO playlistDTO)
        {
            if (playlistDTO == null || string.IsNullOrWhiteSpace(playlistDTO.Name))
                return BadRequest();

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
                return NotFound();

            playlist.Name = playlistDTO.Name;

            _context.Playlists.Update(playlist);
            await _context.SaveChangesAsync();

            // Оновлюємо пісні для плейлиста
            var existingSongs = _context.PlaylistSongs.Where(ps => ps.PlaylistId == id).ToList();
            _context.PlaylistSongs.RemoveRange(existingSongs);

            foreach (var songId in playlistDTO.SongIds)
            {
                _context.PlaylistSongs.Add(new PlaylistSong
                {
                    PlaylistId = playlist.Id,
                    SongId = songId
                });
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Видалити плейлист
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
                return NotFound();

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
