public class PlaylistDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }  // Зробимо Name nullable
    public List<int> SongIds { get; set; } = new List<int>();
}
