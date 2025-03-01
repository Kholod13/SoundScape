public class SongDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TimeSpan Duration { get; set; }
    public List<int> GenreIds { get; set; } = new List<int>();
    public List<int> ArtistIds { get; set; } = new List<int>();
    public int AlbumId { get; set; }
    public string FilePath { get; set; }
}