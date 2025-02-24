public class SongDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TimeSpan Duration { get; set; }
    public string FilePath { get; set; }
    public string Album { get; set; }
    public int AlbumId { get; set; }
    public List<string> Genres { get; set; } = new List<string>();

    // Додано для зв'язку з артистами
    public List<string> Artists { get; set; } = new List<string>();
}
