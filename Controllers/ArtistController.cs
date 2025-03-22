using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Models;
using SoundScape.DTOs;
using System.Threading.Tasks;
using System.Linq;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArtistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/artist
        [HttpPost]
        public async Task<IActionResult> CreateArtist([FromBody] ArtistDTO artistDTO)
        {
            // Перевірка чи передано імя артиста
            if (artistDTO == null || string.IsNullOrWhiteSpace(artistDTO.Name))
                return BadRequest("Artist name is required.");

            // Створення нового артиста
            var artist = new Artist
            {
                Name = artistDTO.Name,
                Bio = artistDTO.Bio
            };

            // Додавання артиста в базу даних
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            // Повернення успішної відповіді з ID артиста
            return CreatedAtAction(nameof(CreateArtist), new { id = artist.Id }, artist);
        }

        // GET: api/artists/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArtistById(int id)
        {
            var artist = await _context.Artists
                .Include(a => a.AlbumArtists) // Завантажуємо зв'язки з альбомами
                .ThenInclude(aa => aa.Album)  // Завантажуємо самі альбоми
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null)
                return NotFound();

            var artistDTO = new ArtistDTO
            {
                Id = artist.Id,
                Name = artist.Name,
                Bio = artist.Bio,
                Albums = artist.AlbumArtists.Select(aa => new AlbumDTO
                {
                    Id = aa.Album.Id,
                    Title = aa.Album.Title,
                    Year = aa.Album.Year,
                    CoverUrl = aa.Album.CoverUrl
                }).ToList() // Додаємо альбоми до DTO
            };

            return Ok(artistDTO);
        }

        // GET: api/artists/{id}/songs
        // GET: api/artists/{id}/songs
        [HttpGet("{id}/songs")]
        public async Task<IActionResult> GetArtistSongs(int id)
        {
            // Перевірка, чи існує артист
            var artistExists = await _context.Artists.AnyAsync(a => a.Id == id);
            if (!artistExists)
                return NotFound($"Artist with ID {id} not found.");

            // Отримання пісень артиста через зв'язки
            var songs = await _context.Songs
                .Where(s => s.Album.AlbumArtists.Any(aa => aa.ArtistId == id))
                .Select(s => new SongDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    // Преобразуем Duration (TimeSpan) в строку, например в формате "hh:mm:ss"
                    Duration = s.Duration.ToString(@"hh\:mm\:ss"),
                    AlbumId = s.AlbumId,
                    AlbumTitle = s.Album.Title,
                    AlbumYear = s.Album.Year,  // Додаємо рік альбому
                    AlbumCoverUrl = s.Album.CoverUrl,  // Додаємо URL обкладинки альбому
                    Lyrics = s.Lyrics  // Добавляем текст песни
                })
                .ToListAsync();

            if (songs == null || songs.Count == 0)
                return NotFound($"No songs found for artist with ID {id}.");

            return Ok(songs);
        }



        // GET: api/artists
        [HttpGet]
        public async Task<IActionResult> GetAllArtists()
        {
            var artists = await _context.Artists
                .Select(a => new ArtistDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Bio = a.Bio
                })
                .ToListAsync();

            return Ok(artists);
        }

        // GET: api/artists/{id}/albums
        [HttpGet("{id}/albums")]
        public async Task<IActionResult> GetArtistAlbums(int id)
        {
            var albums = await _context.Albums
                .Where(a => a.AlbumArtists.Any(aa => aa.ArtistId == id)) // Фільтрація за ArtistId через AlbumArtists
                .Include(a => a.AlbumArtists) // Завантажуємо зв'язки між альбомами та артистами
                .ToListAsync();

            if (albums == null || albums.Count == 0)
                return NotFound(); // Якщо альбоми не знайдені

            // Повернення альбомів з детальною інформацією
            var albumDTOs = albums.Select(a => new AlbumDTO
            {
                Id = a.Id,
                Title = a.Title,
                Year = a.Year,
                CoverUrl = a.CoverUrl
            }).ToList();

            return Ok(albumDTOs); // Повернення альбомів
        }

        // PUT: api/artists/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArtist(int id, [FromBody] ArtistDTO artistDTO)
        {
            if (artistDTO == null || id != artistDTO.Id)
                return BadRequest("Artist ID mismatch.");

            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
                return NotFound();

            // Оновлення інформації артиста
            artist.Name = artistDTO.Name;
            artist.Bio = artistDTO.Bio;

            _context.Artists.Update(artist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/artists/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
                return NotFound();

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}