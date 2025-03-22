using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Models;
using SoundScape.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;

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

        // Створення пісні
        [HttpPost]
        public async Task<IActionResult> CreateSong(
            [FromForm] IFormFile file,
            [FromForm] string title,
            [FromForm] string duration,
            [FromForm] string genreIds,
            [FromForm] string artistIds,
            [FromForm] int albumId,
            [FromForm] string lyrics,
            [FromForm] IFormFile coverImage) // Новий параметр для обкладинки
        {
            try
            {
                // Перевірка типу контенту запиту
                if (!Request.ContentType.StartsWith("multipart/form-data"))
                {
                    return BadRequest("Content-Type must be multipart/form-data.");
                }

                // Перевірка наявності файлу
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File is required.");
                }

                // Перевірка на правильність формату тривалості
                if (!TimeSpan.TryParse(duration, out TimeSpan songDuration))
                {
                    return BadRequest("Invalid duration format.");
                }

                // Обробка списків жанрів і артистів
                var genreList = genreIds.Split(',').Select(int.Parse).ToList();
                var artistList = artistIds.Split(',').Select(int.Parse).ToList();

                // Перевірка наявності альбому
                var album = await _context.Albums.FindAsync(albumId);
                if (album == null)
                {
                    return BadRequest("Album not found.");
                }

                // Збереження файлу пісні і отримання шляху
                var filePath = await SaveFileAsync(file);

                // Збереження обкладинки
                string coverImagePath = null;
                if (coverImage != null && coverImage.Length > 0)
                {
                    coverImagePath = await SaveCoverImageAsync(coverImage);
                }

                // Створення нової пісні
                var song = new Song
                {
                    Title = title,
                    Duration = songDuration,
                    FilePath = filePath,
                    AlbumId = albumId,
                    Lyrics = lyrics,
                    CoverImageUrl = coverImagePath  // Зберігаємо URL обкладинки
                };

                // Додавання жанрів
                foreach (var genreId in genreList)
                {
                    var genre = await _context.Genres.FindAsync(genreId);
                    if (genre != null)
                    {
                        song.SongGenres.Add(new SongGenre { GenreId = genreId });
                    }
                }

                // Додавання артистів
                foreach (var artistId in artistList)
                {
                    var artist = await _context.Artists.FindAsync(artistId);
                    if (artist != null)
                    {
                        song.SongArtists.Add(new SongArtist { ArtistId = artistId });
                    }
                }

                // Збереження пісні в базі даних
                _context.Songs.Add(song);
                await _context.SaveChangesAsync();

                // Повернення результату з інформацією про пісню
                return CreatedAtAction(nameof(GetSongById), new { id = song.Id }, song);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Метод для перетворення локального шляху в URL (для пісень)
        private string ConvertToFileUrl(string filePath, string folderName)
        {
            var fileName = Path.GetFileName(filePath);
            var fileUrl = $"http://localhost:5253/Uploads/{Uri.EscapeDataString(fileName)}"; // Перетворюємо в URL
            return fileUrl;
        }

        // Метод для перетворення локального шляху в URL (для обкладинок пісень)
        private string ConvertCoverImageToFileUrl(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var fileUrl = $"http://localhost:5253/coverImages/{Uri.EscapeDataString(fileName)}"; // Перетворюємо в URL
            return fileUrl;
        }

        // Збереження файлу пісні
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

        // Збереження обкладинки пісні
        private async Task<string> SaveCoverImageAsync(IFormFile coverImage)
        {
            var coverImagesFolder = Path.Combine(_hostEnvironment.WebRootPath, "coverImages");

            if (!Directory.Exists(coverImagesFolder))
            {
                Directory.CreateDirectory(coverImagesFolder);
            }

            var coverImagePath = Path.Combine(coverImagesFolder, coverImage.FileName);
            using (var stream = new FileStream(coverImagePath, FileMode.Create))
            {
                await coverImage.CopyToAsync(stream);
            }

            return coverImagePath;
        }

        // Отримання пісні за ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSongById(int id)
        {
            var song = await _context.Songs
                .Include(s => s.SongGenres).ThenInclude(sg => sg.Genre)
                .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist)
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
                return NotFound();

            var result = new SongDTO
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration.ToString("mm\\:ss"),
                FilePath = ConvertToFileUrl(song.FilePath, "Uploads"),
                AlbumId = song.AlbumId,
                AlbumTitle = song.Album?.Title,
                GenreIds = song.SongGenres?.Select(sg => sg.GenreId).ToList() ?? new List<int>(),
                ArtistIds = song.SongArtists?.Select(sa => sa.Artist.Id).ToList() ?? new List<int>(),
                Artists = song.SongArtists?.Select(sa => sa.Artist.Name).ToList() ?? new List<string>(),
                Lyrics = song.Lyrics,
                AlbumCoverUrl = song.CoverImageUrl != null ? ConvertCoverImageToFileUrl(song.CoverImageUrl) : null // Використовуємо новий метод для URL обкладинок пісень
            };

            return Ok(result);
        }

        // Оновлення слів пісні та обкладинки
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(int id,
            [FromForm] string lyrics,
            [FromForm] IFormFile coverImage)
        {
            try
            {
                // Знайти пісню по ID
                var song = await _context.Songs
                    .Include(s => s.SongGenres)
                    .Include(s => s.SongArtists)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (song == null)
                {
                    return NotFound("Song not found.");
                }

                // Оновлюємо тільки слова пісні, якщо вони передані
                if (!string.IsNullOrEmpty(lyrics))
                {
                    song.Lyrics = lyrics;
                }

                // Якщо передана нова обкладинка, зберігаємо її
                if (coverImage != null && coverImage.Length > 0)
                {
                    song.CoverImageUrl = await SaveCoverImageAsync(coverImage); // Зберігаємо нову обкладинку
                }

                // Збереження змін у базі даних
                _context.Songs.Update(song);
                await _context.SaveChangesAsync();

                return Ok(song); // Повертаємо оновлену пісню
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DTO для пісні
        public class SongDTO
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Duration { get; set; }
            public int AlbumId { get; set; }
            public string AlbumTitle { get; set; }
            public string AlbumCoverUrl { get; set; }
            public string FilePath { get; set; }
            public List<string> Artists { get; set; }
            public string Lyrics { get; set; }

            // Додаємо ці властивості
            public List<int> ArtistIds { get; set; }
            public List<int> GenreIds { get; set; }
        }
    }
}
