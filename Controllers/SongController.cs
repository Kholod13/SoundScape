using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundScape.Models;
using SoundScape.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly ILogger<SongController> _logger;
        private readonly string _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

        public SongController(ISongService songService, ILogger<SongController> logger)
        {
            _songService = songService;
            _logger = logger;
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        // GET: api/song
        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            try
            {
                var songs = await _songService.GetAllSongsAsync();
                return Ok(songs);
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
                    return NotFound();
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateSong([FromForm] SongUploadDto model)

        {
            if (model == null)
            {
                _logger.LogWarning("Received null song data.");
                return BadRequest("Song data cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.Mp3File == null || model.Mp3File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (Path.GetExtension(model.Mp3File.FileName).ToLower() != ".mp3")
            {
                return BadRequest("Only MP3 files are allowed.");
            }

            try
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Mp3File.FileName);
                var filePath = Path.Combine(_uploadFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Mp3File.CopyToAsync(stream);
                }

                var song = new Song
                {
                    Title = model.Title,
                    Artist = model.Artist,
                    Album = model.Album,
                    Duration = (decimal)model.Duration,
                    FilePath = "/Uploads/" + fileName
                };

                _logger.LogInformation($"Adding new song: {song.Title} by {song.Artist}");
                await _songService.AddSongAsync(song);

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
                return BadRequest("ID in request does not match model ID.");
            }

            try
            {
                var existingSong = await _songService.GetSongByIdAsync(id);
                if (existingSong == null)
                {
                    return NotFound();
                }

                await _songService.UpdateSongAsync(song);
                return NoContent();
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
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting song with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class SongUploadDto
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public double Duration { get; set; }
        public IFormFile Mp3File { get; set; }
    }
}
