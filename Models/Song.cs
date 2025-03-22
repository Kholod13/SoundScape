using SoundScape.Models;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TimeSpan Duration { get; set; }
    public string FilePath { get; set; }
    public int AlbumId { get; set; }
    public Album Album { get; set; }
    public List<SongGenre> SongGenres { get; set; } = new List<SongGenre>();
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
    public ICollection<SongArtist> SongArtists { get; set; } = new List<SongArtist>();

    public string Lyrics { get; set; }

    public string CoverImageUrl { get; set; }  // <-- Ось ця властивість

}