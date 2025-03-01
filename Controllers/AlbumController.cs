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
    [Route("api/albums")]
    public class AlbumController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlbumController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получение всех альбомов
        [HttpGet]
        public async Task<IActionResult> GetAllAlbums()
        {
            var albums = await _context.Albums
                .Include(a => a.AlbumGenres)
                    .ThenInclude(ag => ag.Genre) // Включаем жанры альбома
                .Include(a => a.AlbumArtists)
                    .ThenInclude(aa => aa.Artist) // Включаем артистов альбома
                .ToListAsync();

            return Ok(albums);
        }

        // Получение альбома по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbumById(int id)
        {
            var album = await _context.Albums
                .Include(a => a.AlbumGenres)
                    .ThenInclude(ag => ag.Genre) // Включаем жанры альбома
                .Include(a => a.AlbumArtists)
                    .ThenInclude(aa => aa.Artist) // Включаем артистов альбома
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
                return NotFound();

            return Ok(album);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlbum([FromBody] AlbumDTO albumDTO)
        {
            if (albumDTO == null || string.IsNullOrWhiteSpace(albumDTO.Title))
                return BadRequest("Album data is invalid.");

            var album = new Album
            {
                Title = albumDTO.Title,
                Price = albumDTO.Price,
                Year = albumDTO.Year
            };

            // Инициализация коллекций
            album.AlbumGenres = new List<AlbumGenre>();
            album.AlbumArtists = new List<AlbumArtist>();

            // Добавление жанров
            foreach (var genreId in albumDTO.GenreIds)
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre == null)
                {
                    return NotFound($"Genre with Id {genreId} not found.");
                }
                album.AlbumGenres.Add(new AlbumGenre { GenreId = genreId });
            }

            // Добавление артистов
            foreach (var artistId in albumDTO.ArtistIds)
            {
                var artist = await _context.Artists.FindAsync(artistId);
                if (artist == null)
                {
                    return NotFound($"Artist with Id {artistId} not found.");
                }
                album.AlbumArtists.Add(new AlbumArtist { ArtistId = artistId });
            }

            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlbumById), new { id = album.Id }, album);
        }



        // Обновление альбома
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum(int id, [FromBody] AlbumDTO albumDTO)
        {
            if (albumDTO == null || albumDTO.Id != id)
                return BadRequest();

            var album = await _context.Albums.FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
                return NotFound();

            album.Title = albumDTO.Title;
            album.Price = albumDTO.Price;
            album.Year = albumDTO.Year;

            // Обновляем жанры
            album.AlbumGenres.Clear(); // Удаляем старые жанры
            foreach (var genreId in albumDTO.GenreIds)
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre != null)
                {
                    album.AlbumGenres.Add(new AlbumGenre { GenreId = genreId });
                }
            }

            // Обновляем артистов
            album.AlbumArtists.Clear(); // Удаляем старых артистов
            foreach (var artistId in albumDTO.ArtistIds)
            {
                var artist = await _context.Artists.FindAsync(artistId);
                if (artist != null)
                {
                    album.AlbumArtists.Add(new AlbumArtist { ArtistId = artistId });
                }
            }

            _context.Albums.Update(album);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Удаление альбома
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            var album = await _context.Albums.FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
                return NotFound();

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
