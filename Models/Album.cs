using SoundScape.Models;

public class Album
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int Year { get; set; }

    public ICollection<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
    public ICollection<AlbumGenre> AlbumGenres { get; set; }
    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
