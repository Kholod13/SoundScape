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

        // POST: api/artists
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
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null)
                return NotFound();

            var artistDTO = new ArtistDTO
            {
                Id = artist.Id,
                Name = artist.Name,
                Bio = artist.Bio
            };

            return Ok(artistDTO);
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
                .ToListAsync();

            if (albums == null || albums.Count == 0)
                return NotFound(); // Якщо альбоми не знайдені

            return Ok(albums); // Повернення альбомів
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
