using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/songs")]
    public class SongController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SongController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSong(
            [FromForm] IFormFile file,
            [FromForm] string title,
            [FromForm] string duration,
            [FromForm] string genreIds,
            [FromForm] string artistIds,
            [FromForm] int albumId)
        {
            try
            {
                if (!Request.ContentType.StartsWith("multipart/form-data"))
                {
                    return BadRequest("Content-Type must be multipart/form-data.");
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest("File is required.");
                }

                if (!TimeSpan.TryParse(duration, out TimeSpan songDuration))
                {
                    return BadRequest("Invalid duration format.");
                }

                var genreList = genreIds.Split(',').Select(int.Parse).ToList();
                var artistList = artistIds.Split(',').Select(int.Parse).ToList();

                var album = await _context.Albums.FindAsync(albumId);
                if (album == null)
                {
                    return BadRequest("Album not found.");
                }

                var filePath = await SaveFileAsync(file);

                var song = new Song
                {
                    Title = title,
                    Duration = songDuration,
                    FilePath = filePath,
                    AlbumId = albumId
                };

                foreach (var genreId in genreList)
                {
                    if (await _context.Genres.AnyAsync(g => g.Id == genreId))
                    {
                        song.SongGenres.Add(new SongGenre { GenreId = genreId });
                    }
                }

                foreach (var artistId in artistList)
                {
                    if (await _context.Artists.AnyAsync(a => a.Id == artistId))
                    {
                        song.SongArtists.Add(new SongArtist { ArtistId = artistId });
                    }
                }

                _context.Songs.Add(song);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSongById), new { id = song.Id }, song);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSongById(int id)
        {
            var song = await _context.Songs
                .Include(s => s.SongGenres).ThenInclude(sg => sg.Genre)
                .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
                return NotFound();

            var result = new SongDTO
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration,
                FilePath = song.FilePath,
                GenreIds = song.SongGenres.Select(sg => sg.GenreId).ToList(),
                ArtistIds = song.SongArtists.Select(sa => sa.ArtistId).ToList(),
                AlbumId = song.AlbumId
            };

            return Ok(result);
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
