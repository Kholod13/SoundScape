using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Models;
using SoundScape.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/songs")]
    public class SongController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SongController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            var songs = await _context.Songs
                .Include(s => s.Album)  // Включаємо альбом
                .Include(s => s.SongGenres)
                    .ThenInclude(sg => sg.Genre)  // Включаємо жанри пісні
                .Include(s => s.SongArtists)
                    .ThenInclude(sa => sa.Artist)  // Включаємо артистів пісні
                .Select(s => new SongDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Duration = s.Duration,
                    FilePath = s.FilePath,
                    Album = s.Album.Title,  // Відображаємо заголовок альбому
                    AlbumId = s.AlbumId,
                    Genres = s.SongGenres.Select(sg => sg.Genre.Name).ToList(),  // Перетворюємо жанри в список імен
                    Artists = s.SongArtists.Select(sa => sa.Artist.Name).ToList()  // Перетворюємо артистів в список імен
                })
                .ToListAsync();

            return Ok(songs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSongById(int id)
        {
            var song = await _context.Songs
                .Include(s => s.Album)  // Включаємо альбом
                .Include(s => s.SongGenres)
                    .ThenInclude(sg => sg.Genre)  // Включаємо жанри пісні
                .Include(s => s.SongArtists)
                    .ThenInclude(sa => sa.Artist)  // Включаємо артистів пісні
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
                return NotFound();

            var result = new SongDTO
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration,
                FilePath = song.FilePath,
                Album = song.Album.Title,  // Відображаємо заголовок альбому
                AlbumId = song.AlbumId,
                Genres = song.SongGenres.Select(sg => sg.Genre.Name).ToList(),
                Artists = song.SongArtists.Select(sa => sa.Artist.Name).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSong([FromBody] SongDTO songDTO)
        {
            if (songDTO == null || string.IsNullOrWhiteSpace(songDTO.Title))
                return BadRequest();

            var song = new Song
            {
                Title = songDTO.Title,
                Duration = songDTO.Duration,
                FilePath = songDTO.FilePath,
                AlbumId = songDTO.AlbumId
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSongById), new { id = song.Id }, song);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(int id, [FromBody] SongDTO songDTO)
        {
            if (songDTO == null || songDTO.Id != id)
                return BadRequest();

            var song = await _context.Songs.FirstOrDefaultAsync(s => s.Id == id);
            if (song == null)
                return NotFound();

            song.Title = songDTO.Title;
            song.Duration = songDTO.Duration;
            song.FilePath = songDTO.FilePath;
            song.AlbumId = songDTO.AlbumId;
            _context.Songs.Update(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs.FirstOrDefaultAsync(s => s.Id == id);
            if (song == null)
                return NotFound();

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
