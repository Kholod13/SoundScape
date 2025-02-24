public class Artist
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Bio { get; set; }

    public ICollection<SongArtist> SongArtists { get; set; }


    public ICollection<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
}
