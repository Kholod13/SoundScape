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

        // Створити новий плейлист (тільки назва)
        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistDTO playlistDTO)
        {
            if (playlistDTO == null || string.IsNullOrWhiteSpace(playlistDTO.Name))
                return BadRequest("Назва плейлиста обов'язкова");

            var playlist = new Playlist
            {
                Name = playlistDTO.Name
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylistById), new { id = playlist.Id }, playlist);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlaylist(int id, [FromBody] PlaylistDTO playlistDTO)
        {
            if (playlistDTO.SongIds == null || playlistDTO.SongIds.Count == 0)
                return BadRequest("SongIds are required for updating playlist.");

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
                return NotFound();

            // Якщо передано ім'я, оновлюємо його
            if (!string.IsNullOrWhiteSpace(playlistDTO.Name))
            {
                playlist.Name = playlistDTO.Name;
            }

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
