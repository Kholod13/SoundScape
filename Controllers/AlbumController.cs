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

        [HttpGet]
        public async Task<IActionResult> GetAllAlbums()
        {
            var albums = await _context.Albums
                .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Artist)
                .Include(a => a.Songs)
                .ToListAsync();

            var result = albums.Select(a => new AlbumDTO
            {
                Id = a.Id,
                Title = a.Title,
                Price = a.Price,
                Year = a.Year,
                Artists = a.AlbumArtists.Select(aa => aa.Artist.Name).ToList(),
                Songs = a.Songs.Select(s => s.Title).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbumById(int id)
        {
            var album = await _context.Albums
                .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Artist)
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
                return NotFound();

            var result = new AlbumDTO
            {
                Id = album.Id,
                Title = album.Title,
                Price = album.Price,
                Year = album.Year,
                Artists = album.AlbumArtists.Select(aa => aa.Artist.Name).ToList(),
                Songs = album.Songs.Select(s => s.Title).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlbum([FromBody] AlbumDTO albumDTO)
        {
            if (albumDTO == null || string.IsNullOrWhiteSpace(albumDTO.Title))
                return BadRequest();

            var album = new Album
            {
                Title = albumDTO.Title,
                Price = albumDTO.Price,
                Year = albumDTO.Year
            };

            // Додаємо артистів
            foreach (var artistId in albumDTO.ArtistIds)
            {
                var artist = await _context.Artists.FindAsync(artistId);
                if (artist != null)
                {
                    album.AlbumArtists.Add(new AlbumArtist { ArtistId = artistId });
                }
            }

            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlbumById), new { id = album.Id }, album);
        }

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

            _context.Albums.Update(album);
            await _context.SaveChangesAsync();

            return NoContent();
        }

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
