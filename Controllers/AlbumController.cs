using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/albums")]
    public class AlbumController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _baseUrl = "http://localhost:5253/Uploads/";  // Задаємо базовий URL для файлів

        public AlbumController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAlbums()
        {
            var albums = await _context.Albums
                .Include(a => a.AlbumGenres)
                    .ThenInclude(ag => ag.Genre)
                .Include(a => a.AlbumArtists)
                    .ThenInclude(aa => aa.Artist)
                .Include(a => a.Songs)
                    .ThenInclude(s => s.SongArtists)
                        .ThenInclude(sa => sa.Artist) // Включаємо артистів пісень
                .ToListAsync();

            return Ok(albums);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbumById(int id)
        {
            var album = await _context.Albums
                .Include(a => a.AlbumGenres)
                    .ThenInclude(ag => ag.Genre)
                .Include(a => a.AlbumArtists)
                    .ThenInclude(aa => aa.Artist)
                .Include(a => a.Songs)
                    .ThenInclude(s => s.SongArtists)
                        .ThenInclude(sa => sa.Artist) // Включаємо артистів пісень
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
                return NotFound();

            return Ok(album);
        }

        // GET: api/albums/{id}/songs
        [HttpGet("{id}/songs")]
        public async Task<IActionResult> GetSongsForAlbum(int id)
        {
            // Получаем альбом с песнями и артистами
            var album = await _context.Albums
                .Include(a => a.Songs)
                    .ThenInclude(s => s.SongArtists)
                        .ThenInclude(sa => sa.Artist)
                .FirstOrDefaultAsync(a => a.Id == id);

            // Если альбом не найден, возвращаем ошибку
            if (album == null)
                return NotFound();

            // Получаем список песен альбома
            var songs = album.Songs.Select(song => new SongDTO
            {
                Id = song.Id,
                Title = song.Title,
                // Преобразуем Duration (TimeSpan) в строку в формате "hh:mm:ss"
                Duration = song.Duration.ToString(@"hh\:mm\:ss"),
                FilePath = GetFileUrl(song.FilePath),  // Замена локального пути на URL
                AlbumId = song.AlbumId,
                AlbumTitle = song.Album?.Title, // Добавляем название альбома
                AlbumYear = song.Album?.Year,   // Добавляем год альбома
                AlbumCoverUrl = song.Album?.CoverUrl, // Добавляем URL обложки альбома
                Lyrics = song.Lyrics,  // Текст песни
                Artists = song.SongArtists.Select(sa => sa.Artist.Name).ToList()  // Получаем список артистов
            }).ToList();

            return Ok(songs);
        }



        // Метод для заміни локального шляху на URL
        private string GetFileUrl(string filePath)
        {
            var fileName = Path.GetFileName(filePath);  // Отримуємо назву файлу
            return $"{_baseUrl}{Uri.EscapeDataString(fileName)}";  // Формуємо URL для файлу
        }
    }
}
