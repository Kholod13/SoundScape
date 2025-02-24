using SoundScape.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<SongGenre> SongGenres { get; set; } = new List<SongGenre>();
    public ICollection<AlbumGenre> AlbumGenres { get; set; }
}
