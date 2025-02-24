public class PlaylistDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<int> SongIds { get; set; } // Додаємо ідентифікатори пісень
}
