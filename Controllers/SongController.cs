using Microsoft.AspNetCore.Mvc;
using SoundScape.Models;
using SoundScape.Services;
using Microsoft.Extensions.Logging;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly ILogger<SongController> _logger;

        public SongController(ISongService songService, ILogger<SongController> logger)
        {
            _songService = songService;
            _logger = logger;
        }

        // GET: api/song
        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            try
            {
                var songs = await _songService.GetAllSongsAsync();
                return Ok(songs); // Повертаємо список пісень у форматі JSON
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching songs");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/song/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSongById(int id)
        {
            try
            {
                var song = await _songService.GetSongByIdAsync(id);
                if (song == null)
                {
                    return NotFound(); // Якщо пісня не знайдена
                }
                return Ok(song);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching song with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/song
        [HttpPost]
        public async Task<IActionResult> CreateSong([FromBody] Song song)
        {
            if (song == null)
            {
                _logger.LogWarning("Received null song data.");
                return BadRequest("Song data cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Повертаємо помилки валідації
            }

            try
            {
                // Логування для перевірки
                _logger.LogInformation($"Adding new song: {song.Title} by {song.Artist}");

                // Додавання пісні через сервіс
                await _songService.AddSongAsync(song);

                // Повертаємо відповідь із статусом 201 (Created)
                return CreatedAtAction(nameof(GetSongById), new { id = song.Id }, song);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new song");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/song/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(int id, [FromBody] Song song)
        {
            if (id != song.Id)
            {
                return BadRequest("ID в параметрах і моделі не співпадають.");
            }

            try
            {
                var existingSong = await _songService.GetSongByIdAsync(id);
                if (existingSong == null)
                {
                    return NotFound();
                }

                await _songService.UpdateSongAsync(song);
                return NoContent(); // HTTP 204 (успішно, без контенту)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating song with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/song/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            try
            {
                var existingSong = await _songService.GetSongByIdAsync(id);
                if (existingSong == null)
                {
                    return NotFound();
                }

                await _songService.DeleteSongAsync(id);
                return NoContent(); // HTTP 204
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting song with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
