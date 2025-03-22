using System.ComponentModel.DataAnnotations;

public class Artist
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Bio { get; set; }  // Делается необязательным, добавляется ?
    public string? AvatarUrl { get; set; }  // Делается необязательным

    public ICollection<SongArtist> SongArtists { get; set; }

    public ICollection<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
}