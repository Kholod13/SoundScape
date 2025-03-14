using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Models;
using SoundScape.Data;  // Додайте цей using для доступу до SoundScapeDbContext
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class SongController : ControllerBase
{
    private readonly SoundScapeDbContext _context;

    public SongController(SoundScapeDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddSong([FromBody] Song song)
    {
        _context.Songs.Add(song);
        await _context.SaveChangesAsync();
        return Ok(song);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSongs()
    {
        var songs = await _context.Songs.ToListAsync();
        return Ok(songs);
    }
}
